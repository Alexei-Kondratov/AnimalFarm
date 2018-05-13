using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public class CachedRepository<TEntity> : IRepository<TEntity>
    {
        private readonly IRepository<TEntity> _cacheRepository;
        private readonly IRepository<TEntity> _sourceRepository;
        
        public CachedRepository(IRepository<TEntity> cacheRepository,
            IRepository<TEntity> sourceRepository)
        {
            _cacheRepository = cacheRepository;
            _sourceRepository = sourceRepository;
        }

        public async Task<TEntity> ByIdAsync(ITransaction transaction, string id)
        {
            var value = await _cacheRepository.ByIdAsync(transaction, id);
            if (value != null)
                return value;

            value = await _sourceRepository.ByIdAsync(transaction, id);
            await _cacheRepository.UpsertAsync(transaction, value);
            return value;
        }

        public async Task UpsertAsync(ITransaction transaction, TEntity entity)
        {
            await _cacheRepository.UpsertAsync(transaction, entity);
            await _sourceRepository.UpsertAsync(transaction, entity);
        }
    }
}
