using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public interface IDataSource
    {
        string Name { get; }

        TransactionContext CreateTransactionContext();

        Task<TEntity> ByIdAsync<TEntity>(ITransaction transaction, string storeName, string partitionKey, string entityId);

        Task AddOperationAsync<TEntity>(ITransaction transaction, DataOperationType operationType, string storeName, TEntity entity)
            where TEntity : IHavePartition<string, string>;

        Task ComitAsync(ITransaction transaction);
    }
}
