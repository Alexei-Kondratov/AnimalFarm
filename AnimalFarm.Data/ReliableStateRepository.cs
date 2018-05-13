using AnimalFarm.Model;
using System;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public class ReliableStateRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IHaveId<string>
    {
        private IReliableStateManager _stateManager;
        private string _repositoryName;

        public ReliableStateRepository(IReliableStateManager stateManager, string repositoryName = null)
        {
            _repositoryName = repositoryName ?? typeof(TEntity).Name;
            _stateManager = stateManager;
        }

        private async Task<IReliableDictionary<string, TEntity>> GetDictionary()
        {
            return await _stateManager.GetOrAddAsync<IReliableDictionary<string, TEntity>>(_repositoryName);
        }

        public async Task<TEntity> ByIdAsync(ITransaction transaction, string partitionId, string entityId)
        {
            var tx = transaction as IReliableStateTransaction;
            if (tx == null)
                throw new ArgumentException("Expecting a transaction convertable to IReliableStateTransaction", nameof(transaction));

            var reliableDictionary = await GetDictionary();

            var result = await reliableDictionary.TryGetValueAsync(tx.Object, entityId);
            return result.HasValue ? result.Value : null;
        }

        public async Task UpsertAsync(ITransaction transaction, TEntity entity)
        {
            var tx = transaction as IReliableStateTransaction;
            if (tx == null)
                throw new ArgumentException("Expecting a transaction convertable to IReliableStateTransaction", nameof(transaction));

            var reliableDictionary = await GetDictionary();

            await reliableDictionary.SetAsync(tx.Object, entity.Id, entity);
        }
    }
}
