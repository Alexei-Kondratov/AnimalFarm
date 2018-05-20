using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.Data
{
    /// <summary>
    /// Encapsulates access to Azure Cloud Storage.
    /// </summary>
    public class CloudStorageConnector
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;

        public CloudStorageConnector(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        public CloudTable GetTable(string tableName)
        {
            return _tableClient.GetTableReference(tableName);
        }
    }
}
