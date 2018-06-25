using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.Configuration;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Data.Seed
{
    public class SeedConfiguraiton : ISeedData
    {
        public IEnumerable<SeedCollection> Collections { get; }

        public SeedConfiguraiton()
        {
            Collections = new[]
            {
                new SeedCollection
                {
                    Name = "Configuration",
                    Entities = new [] {
                        new ConfigurationRecord("", new BranchConfiguration { ActiveBranchId = SeedData.DefaultBranchId }),
                        new ConfigurationRecord("DatabaseConnection", new DocumentDbConnectionInfo
                        {
                            DatabaseName = "AnimalFarm",
                            Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                            Uri = new Uri("https://localhost:8081")
                        }),
                        new ConfigurationRecord("AnimalService", new DocumentDbDataSourceConfiguration  { Key = "DatabaseDataSource", ConnectionInfoName = "DatabaseConnection" }),
                        new ConfigurationRecord("AnimalService", new ServiceProxyDataSourceConfiguration { Key = "RulesetProxyDataSource", Service = ServiceType.Ruleset }),
                        new ConfigurationRecord("AnimalService", new ReliableStateDataSourceConfiguration { Key = "ReliableStateDataSource" }),
                        new ConfigurationRecord("AnimalService",  new DataSourceRepositoryConfiguration
                                    { Type = typeof(Animal), StoreName = "Animals", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" }),
                        new ConfigurationRecord("AnimalService",  new DataSourceRepositoryConfiguration
                                    { Type = typeof(VersionSchedule), StoreName = "VersionSchedules", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" }),
                        new ConfigurationRecord("AnimalService",  new DataSourceRepositoryConfiguration
                                    { Type = typeof(Ruleset), StoreName = "Rulesets", DataSourceName = "RulesetProxyDataSource", CacheDataSourceName = "ReliableStateDataSource" }),

                        new ConfigurationRecord("RulesetService", new ReliableStateDataSourceConfiguration { Key = "ReliableStateDataSource" }),
                        new ConfigurationRecord("RulesetService", new DocumentDbDataSourceConfiguration
                                {
                                    Key = "DatabaseDataSource",
                                    ConnectionInfoName = "DatabaseConnection",
                                    Decorators = new [] { typeof(RulesetUnpackingDecorator) }
                                }),
                        new ConfigurationRecord("RulesetService", new DataSourceRepositoryConfiguration
                                    { Type = typeof(VersionSchedule), StoreName = "VersionSchedules", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" }),
                        new ConfigurationRecord("RulesetService", new DataSourceRepositoryConfiguration
                                    { Type = typeof(Ruleset), StoreName = "Rulesets", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" })
                    }
                }
            };
        }
    }
}
