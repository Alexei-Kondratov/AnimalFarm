using AnimalFarm.Data.Transactions;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public interface ITransaction : IDisposable
    {
        TTransactionContext GetContext<TTransactionContext>(IDataSource<TTransactionContext> dataSource)
            where TTransactionContext : TransactionContext;

        Task CommitAsync();
    }

}
