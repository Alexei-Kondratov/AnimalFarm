using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Service;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Services.Utils.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.AnimalService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class AnimalService : BaseStatefulService
    {

        public AnimalService(StatefulServiceContext context)
            : base(context)
        {
            EventSource = ServiceEventSource.Current;
        }

        protected override IEnumerable<DataSourceConfiguration> GetDataSourceConfigurations()
        {
            return new DataSourceConfiguration[]
            {
                new DocumentDbDataSourceConfiguration { Key = "DatabaseDataSource" },
                new ServiceProxyDataSourceConfiguration { Key = "RulesetProxyDataSource", Service = ServiceType.Ruleset },
                new ReliableStateDataSourceConfiguration { Key = "ReliableStateDataSource" },
            };
        }

        protected override IEnumerable<RepositoryConfiguration> GetRepositoryConfigurations()
        {
            return new RepositoryConfiguration[]
            {
                new DataSourceRepositoryConfiguration
                    { Key = typeof(Animal), StoreName = "Animals", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" },
                new DataSourceRepositoryConfiguration
                    { Key = typeof(VersionSchedule), StoreName = "VersionSchedules", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" },
                new DataSourceRepositoryConfiguration
                    { Key = typeof(Ruleset), StoreName = "Rulesets", DataSourceName = "RulesetProxyDataSource", CacheDataSourceName = "ReliableStateDataSource" }
            };
        }

        protected override void SetupWebHost(IWebHostBuilder builder)
        {
            base.SetupWebHost(builder);
            builder.UseStartup<Startup>();
        }

        protected override void RegisterServices(IServiceCollection serviceCollection)
        {
            base.RegisterServices(serviceCollection);

            serviceCollection
                .AddSingleton<RulesetScheduleProvider>()
                .AddRepository<Animal>()
                .AddRepository<Ruleset>()
                .AddRepository<VersionSchedule>();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne();
        }
    }
}
