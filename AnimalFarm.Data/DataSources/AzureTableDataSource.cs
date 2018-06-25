using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.Data.DataSources
{
    public class AzureTableDataSource : IDataSource
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;

        public string Name { get; private set; }

        public bool IsReadOnly => true;

        public AzureTableDataSource(string name, string connectionString)
        {
            Name = name;
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        private async Task ApplyOperationAsync(DataOperation operation)
        {
            IHavePartition<string, string> entity = operation.Entity;
            TableOperation tableOperation = null;

            switch (operation.Type)
            {
                case DataOperationType.Upsert:
                    tableOperation = TableOperation.InsertOrReplace((ITableEntity)operation.Entity);
                    break;
                case DataOperationType.Remove:
                    tableOperation = TableOperation.Delete((ITableEntity)operation.Entity);
                    break;
                default:
                    return;
            }

            CloudTable table = _tableClient.GetTableReference(operation.StoreName);
            await table.ExecuteAsync(tableOperation);
            // TODO: Handle a failed execution.
        }

        public TransactionContext CreateTransactionContext()
        {
            return new TransactionContext();
        }

        public async Task<TEntity> ByIdAsync<TEntity>(ITransaction transaction, string storeName, string partitionKey, string entityId)
        {
            MethodInfo retrieveMethod = typeof(TableOperation).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.IsGenericMethod);
            retrieveMethod = retrieveMethod.MakeGenericMethod(typeof(TEntity));
            var operation = (TableOperation)retrieveMethod.Invoke(null, new[] { partitionKey, entityId, null });

            CloudTable table = _tableClient.GetTableReference(storeName);            
            TableResult executionResult = await table.ExecuteAsync(operation);
            // TODO: Handle a failed execution.
            return (TEntity)(executionResult.Result);
        }

        public async Task AddOperationAsync<TEntity>(ITransaction transaction, DataOperationType operationType, string storeName, TEntity entity)
            where TEntity : IHavePartition<string, string>
        {
            transaction.GetContext(this).AddOperation(operationType, storeName, entity);
        }

        public async Task ComitAsync(ITransaction transaction)
        {
            foreach (DataOperation operation in transaction.GetContext(this).Operations)
                await ApplyOperationAsync(operation);
        }
    }
}
