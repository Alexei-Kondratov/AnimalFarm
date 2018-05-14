using System;

namespace AnimalFarm.Service.Utils
{
    public abstract class RepositoryConfiguration
    {
        public string EntityType { get; set; }
        public string LocalCacheName { get; set; }
    }

    public class AzureTableRepositoryConfiguration : RepositoryConfiguration
    {
        public string TableName { get; set; }
    }

    public class ReadOnlyProxyRepositoryConfiguration : RepositoryConfiguration
    {
        public string SourceService { get; set; }
        public string SourceEndpointPath { get; set; }
    }
}
