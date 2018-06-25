using AnimalFarm.Utils.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Cache
{
    public class CacheManager
    {
        private readonly INamedServiceProvider _namedServiceProvider;

        public CacheManager(INamedServiceProvider namedServiceProvider)
        {
            _namedServiceProvider = namedServiceProvider;
        }

        public async Task ClearAllAsync()
        { 
            var clearableRepositories = _namedServiceProvider.GetAllServices<ICachedRepository>();
            var clearTasks = clearableRepositories.Select(r => r.ClearCacheAsync());
            await Task.WhenAll(clearTasks);
        }
    }
}
