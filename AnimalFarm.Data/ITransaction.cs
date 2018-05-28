using AnimalFarm.Data.Transactions;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public interface ITransaction : IDisposable
    {
        TransactionContext GetContext(IDataSource dataSource);
        Task CommitAsync();
    }
}
