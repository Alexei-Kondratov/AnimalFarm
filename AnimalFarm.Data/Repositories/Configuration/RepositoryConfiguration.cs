using AnimalFarm.Utils.Configuration;
using System;

namespace AnimalFarm.Data.Repositories.Configuration
{
    public abstract class RepositoryConfiguration : IComponentConfiguration
    {
        public virtual string Key { get; }
    }

    public class DataSourceRepositoryConfiguration : RepositoryConfiguration
    {
        public override string Key => Type.Name;
        public Type Type { get; set; }
        public string DataSourceName { get; set; }
        public string StoreName { get; set; }
        public string CacheDataSourceName { get; set; }
    }
}
