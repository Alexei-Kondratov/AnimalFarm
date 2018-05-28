using System.Threading.Tasks;
using AnimalFarm.Data.Cache;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace AnimalFarm.Data.DataSources
{
    public class ReliableStateDataSource : IDataSource, IClearable
    {
        private IReliableStateManager _stateManager;

        public string Name { get; private set; }

        public ReliableStateDataSource(string name, IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
            Name = name;
        }

        private async Task<IReliableDictionary<string, TEntity>> GetDictionaryAsync<TEntity>(string storeName)
        {
            return await _stateManager.GetOrAddAsync<IReliableDictionary<string, TEntity>>(storeName);
        }

        public async Task<TEntity> ByIdAsync<TEntity>(ITransaction transaction, string storeName, string partitionKey, string entityId)
        {
            var typedContext = (ReliableStateTransactionContext)transaction.GetContext(this);
            IReliableDictionary<string, TEntity> reliableDictionary = await GetDictionaryAsync<TEntity>(storeName);
            ConditionalValue<TEntity> result = await reliableDictionary.TryGetValueAsync(typedContext.ReliableTransaction, entityId);
            return result.HasValue ? result.Value : default(TEntity);
        }

        public TransactionContext CreateTransactionContext()
        {
            return new ReliableStateTransactionContext(_stateManager.CreateTransaction());
        }

        public async Task ComitAsync(ITransaction transaction)
        {
            var typedContext = (ReliableStateTransactionContext)transaction.GetContext(this);
            await typedContext.ReliableTransaction.CommitAsync();
        }

        public async Task AddOperationAsync<TEntity>(ITransaction transaction, DataOperationType operationType, string storeName, TEntity entity)
            where TEntity : IHavePartition<string, string>
        {
            var typedContext = (ReliableStateTransactionContext)transaction.GetContext(this);

            IReliableDictionary<string, TEntity> reliableDictionary = await GetDictionaryAsync<TEntity>(storeName);
            switch (operationType)
            {
                case DataOperationType.Upsert:
                    await reliableDictionary.AddOrUpdateAsync(typedContext.ReliableTransaction, entity.Id, entity, (key, oldValue) => entity);
                    break;
                case DataOperationType.Remove:
                    await reliableDictionary.TryRemoveAsync(typedContext.ReliableTransaction, entity.Id);
                    break;
            }

            typedContext.AddOperation(operationType, storeName, entity);
        }

        public async Task ClearAsync(string storeName)
        {
            await _stateManager.RemoveAsync(storeName);
        }
    }
}
