using System;
using System.Threading.Tasks;
using AnimalFarm.Data;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;

namespace AnimalFarm.Service.Utils
{
    public class ServiceProxyDataSource : IDataSource
    {
        private readonly ServiceType _serviceType;

        public string Name { get; private set; }

        public bool IsReadOnly => true;

        public ServiceProxyDataSource(string name, ServiceType serviceType)
        {
            Name = name;
            _serviceType = serviceType;
        }

        public Task AddOperationAsync<TEntity>(ITransaction context, DataOperationType upsert, string storeName, TEntity entity) where TEntity : IHavePartition<string, string>
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> ByIdAsync<TEntity>(ITransaction context, string storeName, string partitionKey, string entityId)
        {
            var client = new ServiceHttpClient(_serviceType, partitionKey);
            var formattedPath = $"{storeName}/{entityId}";
            return await client.GetAsync<TEntity>(formattedPath);
        }

        public async Task ComitAsync(ITransaction context)
        {
        }

        public TransactionContext CreateTransactionContext()
        {
            return new TransactionContext();
        }
    }
}
