using System.Collections.Generic;
using System.Threading.Tasks;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace AnimalFarm.Data.DataSources
{
    public class ReliableStateDataSource : IDataSource<ReliableStateTransactionContext>
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

        public async Task<TEntity> ByIdAsync<TEntity>(ReliableStateTransactionContext context, string storeName, string partitionKey, string entityId)
        {
            IReliableDictionary<string, TEntity> reliableDictionary = await GetDictionaryAsync<TEntity>(storeName);
            ConditionalValue<TEntity> result = await reliableDictionary.TryGetValueAsync(context.ReliableTransaction, entityId);
            return result.HasValue ? result.Value : default(TEntity);
        }

        public ReliableStateTransactionContext CreateTransactionContext()
        {
            return new ReliableStateTransactionContext(_stateManager.CreateTransaction());
        }

        public async Task ComitAsync(ReliableStateTransactionContext context)
        {
            await context.ReliableTransaction.CommitAsync();
        }

        public async Task AddOperationAsync<TEntity>(ReliableStateTransactionContext context, DataOperationType operationType, string storeName, TEntity entity)
            where TEntity : IHavePartition<string, string>
        {
            IReliableDictionary<string, TEntity> reliableDictionary = await GetDictionaryAsync<TEntity>(storeName);
            switch (operationType)
            {
                case DataOperationType.Upsert:
                    await reliableDictionary.AddOrUpdateAsync(context.ReliableTransaction, entity.Id, entity, (key, oldValue) => entity);
                    break;
                case DataOperationType.Remove:
                    await reliableDictionary.TryRemoveAsync(context.ReliableTransaction, entity.Id);
                    break;
            }

            context.AddOperation(operationType, storeName, entity);
        }
    }
}
