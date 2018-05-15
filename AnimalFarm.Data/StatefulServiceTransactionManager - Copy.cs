using Microsoft.ServiceFabric.Data;

namespace AnimalFarm.Data
{
    /// <summary>
    /// ITransactionManager implementation suitable for stateful services.
    /// </summary>
    public class StatelessServiceTransactionManager : ITransactionManager
    {
        public StatelessServiceTransactionManager()
        {
        }

        public ITransaction CreateTransaction()
        {
            return new StatelessServiceTransaction();
        }
    }
}
