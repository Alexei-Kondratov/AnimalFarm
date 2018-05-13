using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public class AzureTableRepository<TEntity> : IRepository<TEntity>
        where TEntity : TableEntity
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;

        public AzureTableRepository(string connectionString, string tableName)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference(tableName);
        }

        public async Task<TEntity> ByIdAsync(ITransaction transaction, string partitionId, string entityId)
        {
            var operation = TableOperation.Retrieve<TEntity>(partitionId, entityId);
            var executionResult = await _table.ExecuteAsync(operation);
            // TODO: Handle a failed execution.
            return executionResult.Result as TEntity;
        }

        public async Task UpsertAsync(ITransaction transaction, TEntity entity)
        {
            var operation = TableOperation.InsertOrReplace(entity);
            var executionResult = await _table.ExecuteAsync(operation);
            // TODO: Handle a failed execution.
        }
    }
}
