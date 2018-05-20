using Microsoft.ServiceFabric.Data;

namespace AnimalFarm.Data
{
    /// <summary>
    /// ITransactionManager implementation suitable for stateful services.
    /// </summary>
    public class StatefulServiceTransactionManager : StatelessServiceTransactionManager, ITransactionManager
    {
        private readonly IReliableStateManager _stateManager;

        public StatefulServiceTransactionManager(CloudStorageConnector azureConnector, IReliableStateManager stateManager)
            : base(azureConnector)
        {
            _stateManager = stateManager;
        }

        public override ITransaction CreateTransaction()
        {
            return new StatefulServiceTransaction(_azureConnector, _stateManager.CreateTransaction());
        }
    }
}
