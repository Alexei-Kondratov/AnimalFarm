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
                        new ConfigurationRecord("AnimalService", new DataSourceConfigurations
                        {
                            Configurations = new DataSourceConfiguration[]
                            {
                                new DocumentDbDataSourceConfiguration { Key = "DatabaseDataSource", ConnectionInfoName = "DatabaseConnection" },
                                new ServiceProxyDataSourceConfiguration { Key = "RulesetProxyDataSource", Service = ServiceType.Ruleset },
                                new ReliableStateDataSourceConfiguration { Key = "ReliableStateDataSource" },
                            }
                        }),
                        new ConfigurationRecord("AnimalService", new RepositoryConfigurations
                        {
                            Configurations = new RepositoryConfiguration[]
                            {
                                new DataSourceRepositoryConfiguration
                                    { Key = typeof(Animal), StoreName = "Animals", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" },
                                new DataSourceRepositoryConfiguration
                                    { Key = typeof(VersionSchedule), StoreName = "VersionSchedules", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" },
                                new DataSourceRepositoryConfiguration
                                    { Key = typeof(Ruleset), StoreName = "Rulesets", DataSourceName = "RulesetProxyDataSource", CacheDataSourceName = "ReliableStateDataSource" }
                            }
                        }),
                        new ConfigurationRecord("RulesetService", new DataSourceConfigurations
                        {
                            Configurations = new DataSourceConfiguration[]
                            {
                                new ReliableStateDataSourceConfiguration { Key = "ReliableStateDataSource" },
                                new DocumentDbDataSourceConfiguration
                                {
                                    Key = "DatabaseDataSource",
                                    ConnectionInfoName = "DatabaseConnection",
                                    Decorators = new [] { typeof(RulesetUnpackingDecorator) }
                                }
                            }
                        }),
                        new ConfigurationRecord("RulesetService", new RepositoryConfigurations
                        {
                            Configurations = new[] {
                                new DataSourceRepositoryConfiguration
                                    { Key = typeof(VersionSchedule), StoreName = "VersionSchedules", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" },
                                new DataSourceRepositoryConfiguration
                                    { Key = typeof(Ruleset), StoreName = "Rulesets", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" }
                            }
                        })
                    }
                }
            };
        }
    }
}
