using System;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Repositories
{
    public class TransformingRepositoryDecorator<TEntity> : IRepository<TEntity>
    {
        private IRepository<TEntity> _internalImplementation;
        private IEntityTransformation<TEntity> _transformation;

        public TransformingRepositoryDecorator(IRepository<TEntity> internalImplementation, IEntityTransformation<TEntity> transformation)
        {
            _internalImplementation = internalImplementation;
            _transformation = transformation;
        }

        public async Task<TEntity> ByIdAsync(ITransaction transaction, string partitionId, string id)
        {
            var result = await _internalImplementation.ByIdAsync(transaction, partitionId, id);
            result = await _transformation.TransformAsync(transaction, result);
            return result;
        }

        public Task UpsertAsync(ITransaction transaction, TEntity entity)
        {
            throw new NotSupportedException();
        }
    }
}
