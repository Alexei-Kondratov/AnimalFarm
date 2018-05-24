using System.Threading.Tasks;

namespace AnimalFarm.Data.Repositories
{
    public interface IEntityTransformation<TEntity>
    {
        Task<TEntity> TransformAsync(ITransaction transaction, TEntity entity);
    }
}
