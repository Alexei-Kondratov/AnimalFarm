
namespace AnimalFarm.Data.Transactions
{
    public interface IReliableStateTransaction : ITransaction
    {
        Microsoft.ServiceFabric.Data.ITransaction Object { get; }
    }
}
