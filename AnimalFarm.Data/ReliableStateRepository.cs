using AnimalFarm.Model;
using System;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    /// <summary>
    /// Implements IRepository<TEntity> as a wrapper around IReliableStateManager.
    /// </summary>
    /// <remarks>Expects that transactions implement the IReliableStateTransaction interface.</remarks>
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

        public async Task<TEntity> ByIdAsync(IReliableStateTransaction transaction, string partitionId, string entityId)
        {
            var reliableDictionary = await GetDictionary();
            var result = await reliableDictionary.TryGetValueAsync(transaction.Object, entityId);
            return result.HasValue ? result.Value : null;
        }

        public async Task UpsertAsync(IReliableStateTransaction transaction, TEntity entity)
        {
            var reliableDictionary = await GetDictionary();
            await reliableDictionary.AddOrUpdateAsync(transaction.Object, entity.Id, entity, (key, oldValue) => entity);
        }

        #region Interface implementation: IRepository<IEntity>

        async Task<TEntity> IRepository<TEntity>.ByIdAsync(ITransaction transaction, string partitionId, string entityId)
        {
            if (transaction is IReliableStateTransaction tx)
                return await ByIdAsync(tx, partitionId, entityId);
            else
                throw new ArgumentException("Expecting a transaction implementing IReliableStateTransaction", nameof(transaction));
        }

        async Task IRepository<TEntity>.UpsertAsync(ITransaction transaction, TEntity entity)
        {
            if (transaction is IReliableStateTransaction tx)
                await UpsertAsync(tx, entity);
            else
                throw new ArgumentException("Expecting a transaction implementing IReliableStateTransaction", nameof(transaction));
        }

        #endregion Interface implementation: IRepository<IEntity>
    }
}
