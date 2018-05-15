using System.Threading.Tasks;
using IMsServiceFabricTransaction = Microsoft.ServiceFabric.Data.ITransaction;

namespace AnimalFarm.Data
{
    /// <summary>
    /// ITransaction implementation suitable for stateful services.
    /// </summary>
    public class StatefulServiceTransaction : IAzureTableTransaction, IReliableStateTransaction
    {
        private readonly IMsServiceFabricTransaction _reliableStateTransaction;

        public StatefulServiceTransaction(IMsServiceFabricTransaction reliableStateTransaction)
        {
            _reliableStateTransaction = reliableStateTransaction;
        }

        public IMsServiceFabricTransaction Object => _reliableStateTransaction;

        public async Task CommitAsync()
        {
            await _reliableStateTransaction.CommitAsync();
        }

        public void Dispose()
        {
            _reliableStateTransaction.Dispose();
        }
    }
}
