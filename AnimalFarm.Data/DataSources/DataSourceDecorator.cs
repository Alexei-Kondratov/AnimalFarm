using System.Threading.Tasks;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;

namespace AnimalFarm.Data.DataSources
{
    public class DataSourceDecoratorBase : IDataSource
    {
        protected readonly IDataSource _internal;

        public DataSourceDecoratorBase(IDataSource internalImplementation)
        {
            _internal = internalImplementation;
        }

        public string Name => _internal.Name;

        public bool IsReadOnly => _internal.IsReadOnly;

        public virtual Task AddOperationAsync<TEntity>(ITransaction transaction, DataOperationType operationType, string storeName, TEntity entity) where TEntity : IHavePartition<string, string>
        {
            return _internal.AddOperationAsync(transaction, operationType, storeName, entity);
        }

        public virtual Task<TEntity> ByIdAsync<TEntity>(ITransaction transaction, string storeName, string partitionKey, string entityId)
        {
            return _internal.ByIdAsync<TEntity>(transaction, storeName, partitionKey, entityId);
        }

        public virtual Task ComitAsync(ITransaction transaction)
        {
            return _internal.ComitAsync(transaction);
        }

        public virtual TransactionContext CreateTransactionContext()
        {
            return _internal.CreateTransactionContext();
        }
    }
}
