using AnimalFarm.Data.DataSources;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Repositories
{
    /// <summary>
    /// Implements IRepository as a wrapper around an Azure Storage Table. 
    /// </summary>
    public class DataSourceRepository<TEntity, TDataSource, TTransactionContext> : IRepository<TEntity>
        where TDataSource : IDataSource<TTransactionContext>
        where TTransactionContext : TransactionContext
        where TEntity : IHavePartition<string, string>
    {
        private readonly string _storeName;
        private readonly TDataSource _dataSource;

        public DataSourceRepository(TDataSource dataSource, string storeName)
        {
            _dataSource = dataSource;
            _storeName = storeName;
        }

        async Task<TEntity> IRepository<TEntity>.ByIdAsync(ITransaction transaction, string partitionKey, string entityId)
        {
            TTransactionContext context = transaction.GetContext(_dataSource);
            return await _dataSource.ByIdAsync<TEntity>(context, _storeName, partitionKey, entityId);
        }

        async Task IRepository<TEntity>.UpsertAsync(ITransaction transaction, TEntity entity)
        {
            TTransactionContext context = transaction.GetContext(_dataSource);
            await _dataSource.AddOperationAsync(context, DataOperationType.Upsert, _storeName, entity);
        }
    }
}
