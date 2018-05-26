using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Transactions
{
    public class DataSourceTransaction : ITransaction
    {
        private Dictionary<object, TransactionContext> _contexts = new Dictionary<object, TransactionContext>();

        public TReturnedTransactionContext GetContext<TReturnedTransactionContext>(IDataSource<TReturnedTransactionContext> dataSource)
            where TReturnedTransactionContext : TransactionContext
        {
            if (_contexts.TryGetValue(dataSource, out TransactionContext context))
                return (TReturnedTransactionContext)context;

            TReturnedTransactionContext newContext = dataSource.CreateTransactionContext();
            _contexts.Add(dataSource, newContext);
            return newContext;
        }

        public async Task CommitAsync()
        {
            foreach (KeyValuePair<object, TransactionContext> contextRecord in _contexts)
            {
                object dataSource = contextRecord.Key;
                TransactionContext context = contextRecord.Value;
                MethodInfo commitMethodInfo = dataSource.GetType().GetMethod(nameof(IDataSource<TransactionContext>.ComitAsync));
                var commitResult = (Task)commitMethodInfo.Invoke(dataSource, new[] { context });
                await commitResult;
            }
        }

        public void Dispose()
        {
            foreach (IDisposable context in _contexts.Values)
                context.Dispose();
        }
    }
}
