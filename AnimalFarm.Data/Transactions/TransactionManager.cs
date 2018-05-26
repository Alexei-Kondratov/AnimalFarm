using Microsoft.ServiceFabric.Data;

namespace AnimalFarm.Data.Transactions
{
    /// <summary>
    /// ITransactionManager implementation
    /// </summary>
    public class TransactionManager : ITransactionManager
    {
        public ITransaction CreateTransaction()
        {
            return new DataSourceTransaction();
        }
    }
}
