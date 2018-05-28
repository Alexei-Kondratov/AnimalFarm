using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Repositories
{
    /// <summary>
    /// Implements IRepository as a wrapper around an Azure Storage Table. 
    /// </summary>
    public class DataSourceRepository<TEntity> : IRepository<TEntity>
        where TEntity : IHavePartition<string, string>
    {
        private readonly string _storeName;
        private readonly IDataSource _dataSource;

        public DataSourceRepository(IDataSource dataSource, string storeName)
        {
            _dataSource = dataSource;
            _storeName = storeName;
        }

        async Task<TEntity> IRepository<TEntity>.ByIdAsync(ITransaction transaction, string partitionKey, string entityId)
        {
            return await _dataSource.ByIdAsync<TEntity>(transaction, _storeName, partitionKey, entityId);
        }

        async Task IRepository<TEntity>.UpsertAsync(ITransaction transaction, TEntity entity)
        {
            await _dataSource.AddOperationAsync(transaction, DataOperationType.Upsert, _storeName, entity);
        }
    }
}
