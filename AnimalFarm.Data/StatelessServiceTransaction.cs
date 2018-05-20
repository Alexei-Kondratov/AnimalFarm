using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.Data
{
    /// <summary>
    /// ITransaction implementation suitable for stateless services.
    /// </summary>
    public class StatelessServiceTransaction : IAzureTableTransaction
    {
        private class AzureTableOperationRecord
        {
            public AzureTableOperationRecord(string tableName, TableOperation operation)
            {
                TableName = tableName;
                Operation = operation;
            }

            public string TableName { get; set; }
            public TableOperation Operation { get; set; }
        }

        private CloudStorageConnector _connector;
        private List<AzureTableOperationRecord> _operations = new List<AzureTableOperationRecord>();

        public StatelessServiceTransaction(CloudStorageConnector connector)
        {
            _connector = connector;
        }

        public void AddOperation(string tableName, TableOperation operation)
        {
            _operations.Add(new AzureTableOperationRecord(tableName, operation));
        }

        public virtual async Task CommitAsync()
        {
            if (!_operations.Any())
                return;

            foreach (var operation in _operations)
            {
                CloudTable table = _connector.GetTable(operation.TableName);
                await table.ExecuteAsync(operation.Operation);
            }

            _operations.Clear();
        }

        public virtual void Dispose()
        {
        }
    }
}
