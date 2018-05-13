using Microsoft.ServiceFabric.Data;

namespace AnimalFarm.Data
{
    public class UnifiedTransactionManager : ITransactionManager
    {
        private readonly IReliableStateManager _stateManager;

        public UnifiedTransactionManager(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public ITransaction CreateTransaction()
        {
            return new UnifiedTransaction(_stateManager.CreateTransaction());
        }
    }
}
