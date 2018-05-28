using AnimalFarm.Data;
using AnimalFarm.Data.DataSources.Configuration;
using System;

namespace AnimalFarm.Service.Utils.Configuration
{
    public class ServiceProxyDataSourceBuilder : DataSourceBuilder<ServiceProxyDataSourceConfiguration>
    {
        protected override IDataSource BuildBase(ServiceProxyDataSourceConfiguration configuration, IServiceProvider serviceContainer)
        {
            return new ServiceProxyDataSource(configuration.Key, configuration.Service);
        }
    }
}
