using AnimalFarm.Utils.Configuration;
using System;

namespace AnimalFarm.Data.Repositories.Configuration
{
    public abstract class RepositoryConfiguration : IComponentConfiguration<Type>
    {
        public Type Key { get; set; }
    }

    public class DataSourceRepositoryConfiguration : RepositoryConfiguration
    {
        public string DataSourceName { get; set; }
        public string StoreName { get; set; }
        public string CacheDataSourceName { get; set; }
    }
}
