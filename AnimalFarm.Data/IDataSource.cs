using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public interface IDataSource<TTransactionContext>
        where TTransactionContext : TransactionContext
    {
        string Name { get; }

        TTransactionContext CreateTransactionContext();

        Task<TEntity> ByIdAsync<TEntity>(TTransactionContext context, string storeName, string partitionKey, string entityId);

        Task AddOperationAsync<TEntity>(TTransactionContext context, DataOperationType upsert, string storeName, TEntity entity)
            where TEntity : IHavePartition<string, string>;

        Task ComitAsync(TTransactionContext context);
    }
}
