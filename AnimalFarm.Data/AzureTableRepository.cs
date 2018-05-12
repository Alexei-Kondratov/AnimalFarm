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
        private readonly string _partitionName;

        public AzureTableRepository(string connectionString, string tableName, string partitionName)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference(tableName);
            _partitionName = partitionName;
        }

        public async Task<TEntity> ByIdAsync(string id)
        {
            var operation = TableOperation.Retrieve<TEntity>(_partitionName, id);
            var executionResult = await _table.ExecuteAsync(operation);
            // TODO: Handle failed execution.
            return executionResult.Result as TEntity;
        }

        public async void Upsert(TEntity entity)
        {
            var operation = TableOperation.InsertOrReplace(entity);
            var executionResult = await _table.ExecuteAsync(operation);
            // TODO: Handle failed execution.
        }
    }
}
