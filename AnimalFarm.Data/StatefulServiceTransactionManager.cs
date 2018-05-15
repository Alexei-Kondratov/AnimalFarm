using Microsoft.ServiceFabric.Data;

namespace AnimalFarm.Data
{
    /// <summary>
    /// ITransactionManager implementation suitable for stateful services.
    /// </summary>
    public class StatefulServiceTransactionManager : ITransactionManager
    {
        private readonly IReliableStateManager _stateManager;

        public StatefulServiceTransactionManager(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public ITransaction CreateTransaction()
        {
            return new StatefulServiceTransaction(_stateManager.CreateTransaction());
        }
    }
}
