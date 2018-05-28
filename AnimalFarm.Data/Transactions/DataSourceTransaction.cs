using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Transactions
{
    public class DataSourceTransaction : ITransaction
    {
        private Dictionary<IDataSource, TransactionContext> _contexts = new Dictionary<IDataSource, TransactionContext>();

        public TransactionContext GetContext(IDataSource dataSource)
        {
            if (_contexts.TryGetValue(dataSource, out TransactionContext context))
                return context;

            TransactionContext newContext = dataSource.CreateTransactionContext();
            _contexts.Add(dataSource, newContext);
            return newContext;
        }

        public async Task CommitAsync()
        {
            foreach (IDataSource dataSource in _contexts.Keys)
            {
                await dataSource.ComitAsync(this);
            }
        }

        public void Dispose()
        {
            foreach (IDisposable context in _contexts.Values)
                context.Dispose();
        }
    }
}
