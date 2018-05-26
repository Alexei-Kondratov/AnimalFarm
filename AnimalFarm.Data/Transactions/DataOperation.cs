using AnimalFarm.Model;

namespace AnimalFarm.Data.Transactions
{
    public class DataOperation
    {
        public IHavePartition<string, string> Entity { get; set; }
        public string StoreName { get; set; }
        public DataOperationType Type { get; set; }

        public DataOperation(DataOperationType operationType, string storeName, IHavePartition<string, string> entity)
        {
            Type = operationType;
            StoreName = storeName;
            Entity = entity;
        }
    }
}
