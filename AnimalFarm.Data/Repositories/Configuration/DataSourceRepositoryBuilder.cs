using AnimalFarm.Model;
using AnimalFarm.Utils.Configuration;
using AnimalFarm.Utils.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Repositories.Configuration
{
    public class DataSourceRepositoryBuilder : ConfigurationTypedComponentBuilder<DataSourceRepositoryConfiguration, object, Type>
    {
        private MethodInfo _buildMethodInfo;

        private static async Task<IRepository<TEntity>> BuildTypedResult<TEntity>(DataSourceRepositoryConfiguration configuration, IServiceProvider serviceContainer, INamedServiceProvider namedServiceProvider)
            where TEntity : IHavePartition<string, string>
        {
            var dataSource = namedServiceProvider.GetServiceAsync<IDataSource>(configuration.DataSourceName).GetAwaiter().GetResult();
            IRepository<TEntity> result = new DataSourceRepository<TEntity>(dataSource, configuration.StoreName);

            if (configuration.CacheDataSourceName != null)
            {
                var cacheDataSource = namedServiceProvider.GetServiceAsync<IDataSource>(configuration.CacheDataSourceName).GetAwaiter().GetResult();
                IRepository<TEntity> cache = new DataSourceRepository<TEntity>(cacheDataSource, configuration.StoreName);
                result = new CachedRepository<TEntity>(cache, result);
            }

            return result;
        }

        public DataSourceRepositoryBuilder()
        {
            _buildMethodInfo = GetType().GetMethod(nameof(BuildTypedResult), BindingFlags.NonPublic | BindingFlags.Static);
        }

        protected override object Build(DataSourceRepositoryConfiguration configuration, IServiceProvider serviceContainer)
        {
            return _buildMethodInfo.MakeGenericMethod(configuration.Type).Invoke(null, new object[] { configuration, serviceContainer });
        }
    }
}
