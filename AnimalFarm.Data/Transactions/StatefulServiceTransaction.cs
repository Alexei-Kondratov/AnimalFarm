using System;
using System.Threading.Tasks;
using IMsServiceFabricTransaction = Microsoft.ServiceFabric.Data.ITransaction;

namespace AnimalFarm.Data.Transactions
{
    /// <summary>
    /// ITransaction implementation suitable for stateful services.
    /// </summary>
    public class StatefulServiceTransaction : StatelessServiceTransaction, IReliableStateTransaction
    {
        private readonly IMsServiceFabricTransaction _reliableStateTransaction;

        public StatefulServiceTransaction(CloudStorageConnector azureConnector, IMsServiceFabricTransaction reliableStateTransaction)
            : base(azureConnector)
        {
            _reliableStateTransaction = reliableStateTransaction;
        }

        public IMsServiceFabricTransaction Object => _reliableStateTransaction;

        public override async Task CommitAsync()
        {
            Action<Task> commitReliableState = async (t) => await _reliableStateTransaction.CommitAsync();
            await base.CommitAsync().ContinueWith(commitReliableState);
        }

        public override void Dispose()
        {
            base.Dispose();
            _reliableStateTransaction.Dispose();
        }
    }
}
