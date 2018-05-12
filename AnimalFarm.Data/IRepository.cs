using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> ByIdAsync(string id);
        Task UpsertAsync(TEntity entity);
    }
}
