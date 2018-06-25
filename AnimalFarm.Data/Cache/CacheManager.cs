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
            // TODO: Implement for the new NamesServiceProvicder. 
            //var clearableRepositories = _namedServiceProvider.GetAllServices(typeof(ReliableCacheRepository<>));
            //var clearTasks = clearableRepositories.Select(r => r.ClearAsync());
            //await Task.WhenAll(clearTasks);
        }
    }
}
