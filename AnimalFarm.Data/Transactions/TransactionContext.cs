using AnimalFarm.Model;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Data.Transactions
{
    public class TransactionContext : IDisposable
    {
        private readonly List<DataOperation> _operations = new List<DataOperation>();

        public IEnumerable<DataOperation> Operations => _operations;

        public void AddOperation(DataOperationType operationType, string storeName, IHavePartition<string, string> entity)
        {
            _operations.Add(new DataOperation(operationType, storeName, entity));
        }

        public virtual void Dispose()
        {
        }
    }
}
