using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.Data.DataSources
{
    public class AzureTableDataSource : IDataSource<TransactionContext>
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;

        public string Name { get; private set; }

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

        public async Task<TEntity> ByIdAsync<TEntity>(TransactionContext context, string storeName, string partitionKey, string entityId)
        {
            MethodInfo retrieveMethod = typeof(TableOperation).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.IsGenericMethod);
            retrieveMethod = retrieveMethod.MakeGenericMethod(typeof(TEntity));
            TableOperation operation = (TableOperation)retrieveMethod.Invoke(null, new[] { partitionKey, entityId, null });

            CloudTable table = _tableClient.GetTableReference(storeName);            
            TableResult executionResult = await table.ExecuteAsync(operation);
            // TODO: Handle a failed execution.
            return (TEntity)(executionResult.Result);
        }

        public async Task AddOperationAsync<TEntity>(TransactionContext context, DataOperationType operationType, string storeName, TEntity entity)
            where TEntity : IHavePartition<string, string>
        {
            context.AddOperation(operationType, storeName, entity);
        }

        public async Task ComitAsync(TransactionContext context)
        {
            foreach (DataOperation operation in context.Operations)
                await ApplyOperationAsync(operation);
        }
    }
}
