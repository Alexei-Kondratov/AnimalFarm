using AnimalFarm.Data.Cache;
using AnimalFarm.Model;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Repositories
{
    public class CachedRepository<TEntity> : IRepository<TEntity>
        where TEntity : IHavePartition<string, string>
    {
        private readonly IRepository<TEntity> _cacheRepository;
        private readonly IRepository<TEntity> _sourceRepository;
        
        public CachedRepository(IRepository<TEntity> cacheRepository,
            IRepository<TEntity> sourceRepository)
        {
            _cacheRepository = cacheRepository;
            _sourceRepository = sourceRepository;
        }

        public bool IsReadOnly => _sourceRepository.IsReadOnly;

        public async Task<TEntity> ByIdAsync(ITransaction transaction, string partitionId, string entityId)
        {
            var value = await _cacheRepository.ByIdAsync(transaction, partitionId, entityId);
            if (value != null)
                return value;

            value = await _sourceRepository.ByIdAsync(transaction, partitionId, entityId);

            if (value != null)
                await _cacheRepository.UpsertAsync(transaction, value);

            return value;
        }

        public async Task UpsertAsync(ITransaction transaction, TEntity entity)
        {
            if (IsReadOnly)
                throw new NotSupportedException();

            await _cacheRepository.UpsertAsync(transaction, entity);
            await _sourceRepository.UpsertAsync(transaction, entity);
        }
    }
}
