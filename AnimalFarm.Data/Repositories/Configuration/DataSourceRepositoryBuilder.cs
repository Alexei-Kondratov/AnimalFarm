using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Model;
using AnimalFarm.Utils.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace AnimalFarm.Data.Repositories.Configuration
{
    public class DataSourceRepositoryBuilder : ConfigurationTypedComponentBuilder<DataSourceRepositoryConfiguration, object, Type>
    {
        private MethodInfo _buildMethodInfo;

        private static IRepository<TEntity> BuildDataSourceRepository<TEntity>(string dataSourceName, string storeName, DataSourceFactory dataSourceFactory)
            where TEntity : IHavePartition<string, string>
        {
            IDataSource dataSource = dataSourceFactory.Get(dataSourceName);
            return new DataSourceRepository<TEntity>(dataSource, storeName);
        }

        private static IRepository<TEntity> BuildTypedResult<TEntity>(DataSourceRepositoryConfiguration configuration, IServiceProvider serviceContainer)
            where TEntity : IHavePartition<string, string>
        {
            DataSourceFactory dataSourceFactory = serviceContainer.GetRequiredService<DataSourceFactory>();
            IRepository<TEntity> result = BuildDataSourceRepository<TEntity>(configuration.DataSourceName, configuration.StoreName, dataSourceFactory);

            if (configuration.CacheDataSourceName != null)
            {
                IRepository<TEntity> cache = BuildDataSourceRepository<TEntity>(configuration.CacheDataSourceName, configuration.StoreName, dataSourceFactory);
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
            return _buildMethodInfo.MakeGenericMethod(configuration.Key).Invoke(null, new object[] { configuration, serviceContainer });
        }
    }
}
