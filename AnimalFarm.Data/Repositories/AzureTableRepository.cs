using AnimalFarm.Data.Transactions;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Repositories
{
    /// <summary>
    /// Implements IRepository as a wrapper around an Azure Storage Table. 
    /// </summary>
    public class AzureTableRepository<TEntity> : IRepository<TEntity>
        where TEntity : TableEntity
    {
        private readonly CloudStorageConnector _connector;
        private readonly string _tableName;

        public AzureTableRepository(CloudStorageConnector connector, string tableName)
        {
            _connector = connector;
            _tableName = tableName;
        }

        public async Task<TEntity> ByIdAsync(ITransaction transaction, string partitionId, string entityId)
        {
            var operation = TableOperation.Retrieve<TEntity>(partitionId, entityId);
            var executionResult = await _connector.GetTable(_tableName).ExecuteAsync(operation);
            // TODO: Handle a failed execution.
            return executionResult.Result as TEntity;
        }

        public async Task UpsertAsync(IAzureTableTransaction transaction, TEntity entity)
        {
            var operation = TableOperation.InsertOrReplace(entity);
            transaction.AddOperation(_tableName, operation);
        }

        #region Interface implementation: IRepository

        async Task IRepository<TEntity>.UpsertAsync(ITransaction transaction, TEntity entity)
        {
            if (transaction is IAzureTableTransaction tx)
                await UpsertAsync(tx, entity);
            else
                throw new ArgumentException("Expecting a transaction implementing IAzureTableTransaction", nameof(transaction));
        }

        #endregion Interface implementation: IRepository
    }
}
