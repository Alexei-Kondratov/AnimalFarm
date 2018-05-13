using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> ByIdAsync(ITransaction transaction, string id);
        Task UpsertAsync(ITransaction transaction, TEntity entity);
    }
}
