using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Cache
{
    public class CacheManager
    {
        private readonly DataSourceFactory _dataSources;
        private readonly RepositoryFactory _repositories;

        public CacheManager(DataSourceFactory dataSources, RepositoryFactory repositories)
        {
            _dataSources = dataSources;
            _repositories = repositories;
        }

        public async Task ClearAllAsync()
        {
            foreach (var repository in _repositories.Configurations.OfType<DataSourceRepositoryConfiguration>())
            {
                if (repository.CacheDataSourceName == null)
                    continue;

                IDataSource dataSource = _dataSources.Get(repository.CacheDataSourceName);
                if (dataSource is IClearable clearableDataSource)
                    await clearableDataSource.ClearAsync(repository.StoreName);
            }
        }
    }
}
