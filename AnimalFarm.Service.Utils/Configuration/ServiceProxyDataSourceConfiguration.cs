using AnimalFarm.Data.DataSources.Configuration;

namespace AnimalFarm.Service.Utils.Configuration
{
    public class ServiceProxyDataSourceConfiguration : DataSourceConfiguration
    {
        public ServiceType Service { get; set; }
    }
}
