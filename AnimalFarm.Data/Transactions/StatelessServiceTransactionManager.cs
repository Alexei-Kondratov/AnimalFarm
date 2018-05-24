using Microsoft.ServiceFabric.Data;

namespace AnimalFarm.Data.Transactions
{
    /// <summary>
    /// ITransactionManager implementation suitable for stateful services.
    /// </summary>
    public class StatelessServiceTransactionManager : ITransactionManager
    {
        protected readonly CloudStorageConnector _azureConnector;

        public StatelessServiceTransactionManager(CloudStorageConnector azureConnector)
        {
            _azureConnector = azureConnector;
        }

        public virtual ITransaction CreateTransaction()
        {
            return new StatelessServiceTransaction(_azureConnector);
        }
    }
}
