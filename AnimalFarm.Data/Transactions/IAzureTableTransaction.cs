using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.Data.Transactions
{
    public interface IAzureTableTransaction : ITransaction
    {
        void AddOperation(string tableName, TableOperation operation);
    }
}
