using IReliableStateTransaction = Microsoft.ServiceFabric.Data.ITransaction;

namespace AnimalFarm.Data.Transactions
{
    public class ReliableStateTransactionContext : TransactionContext
    {
        public IReliableStateTransaction ReliableTransaction { get; private set;}

        public ReliableStateTransactionContext(IReliableStateTransaction reliableTransaction)
        {
            ReliableTransaction = reliableTransaction;
        }

        public override void Dispose()
        {
            base.Dispose();
            ReliableTransaction?.Dispose();
        }

    }
}
