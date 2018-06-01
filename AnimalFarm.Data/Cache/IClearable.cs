
using System.Threading.Tasks;

namespace AnimalFarm.Data.Cache
{
    public interface IClearable
    {
        Task ClearAsync(params string[] storeName);
    }
}
