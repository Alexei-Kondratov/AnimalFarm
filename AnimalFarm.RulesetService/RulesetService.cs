using AnimalFarm.Data;
using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Service;
using AnimalFarm.Service.Utils.Tracing;
using AnimalFarm.Services.Utils.DependencyInjection;
using AnimalFarm.Utils.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.RulesetService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class RulesetService : BaseStatefulService
    {
        public RulesetService(StatefulServiceContext context)
            : base(context)
        {
        }

        protected override IEnumerable<DataSourceConfiguration> GetDataSourceConfigurations()
        {
            return new DataSourceConfiguration[]
            {
                new ReliableStateDataSourceConfiguration { Key = "ReliableStateDataSource" },
                new DocumentDbDataSourceConfiguration
                    { Key = "DatabaseDataSource", Decorators = new [] { typeof(RulesetUnpackingDecorator) } }
            };
        }

        protected override IEnumerable<RepositoryConfiguration> GetRepositoryConfigurations()
        {
            return new RepositoryConfiguration[]
            {
                new DataSourceRepositoryConfiguration
                    { Key = typeof(VersionSchedule), StoreName = "VersionSchedules", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" },
                new DataSourceRepositoryConfiguration
                    { Key = typeof(Ruleset), StoreName = "Rulesets", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" }
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
                .AddSingleton<RulesetUnpacker>()
                .AddSingleton<RulesetUnpackingDecorator>()
                .AddRepository<Ruleset>()
                .AddRepository<VersionSchedule>();
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        private async Task PreloadRulesets()
        {
            var transactionManager = ServiceProvider.GetRequiredService<ITransactionManager>();
            var scheduleProvider = ServiceProvider.GetRequiredService<RulesetScheduleProvider>();
            var rulesetRepository = ServiceProvider.GetRequiredService<IRepository<Ruleset>>();

            using (var tx = transactionManager.CreateTransaction())
            {
                var rulesetId = (await scheduleProvider.GetActiveRulesetRecordAsync(tx, DateTime.UtcNow)).RulesetId;
                var ruleset = await rulesetRepository.ByIdAsync(tx, rulesetId, rulesetId);
                ServiceEventSource.Current.ServiceMessage(Context, $"Preloaded {ruleset.Id}");
            }
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await RunTask.WithRetries(PreloadRulesets, 4, cancellationToken);
            cancellationToken.WaitHandle.WaitOne();
        }
    }
}
