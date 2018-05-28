using AnimalFarm.Data;
using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Utils.Configuration;
using AnimalFarm.Utils.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.RulesetService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class RulesetService : StatefulService
    {
        private IServiceProvider _serviceProvider;

        public RulesetService(StatefulServiceContext context)
            : base(context)
        {
        }

        private DataSourceFactory CreateDataSourceFactory(IServiceProvider serviceProvider)
        {
            var result = new DataSourceFactory(_serviceProvider, new IConfigurableComponentBuilder<IDataSource, string>[] { new ReliableStateDataSourceBuilder(), new DocumentDbDataSourceBuilder() });
            result.SetConfigurations(new DataSourceConfiguration[]
            {
                new ReliableStateDataSourceConfiguration { Key = "ReliableStateDataSource" },
                new DocumentDbDataSourceConfiguration
                    { Key = "DatabaseDataSource", Decorators = new [] { typeof(RulesetUnpackingDecorator) } }
            });
            return result;
        }

        private RepositoryFactory CreateRepositoryFactory(IServiceProvider serviceProvider)
        {
            var result = new RepositoryFactory(_serviceProvider, new IConfigurableComponentBuilder<object, Type>[] { new DataSourceRepositoryBuilder() });

            result.SetConfigurations(new RepositoryConfiguration[]
            {
                new DataSourceRepositoryConfiguration
                    { Key = typeof(VersionSchedule), StoreName = "VersionSchedules", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" },
                new DataSourceRepositoryConfiguration
                    { Key = typeof(Ruleset), StoreName = "Rulesets", DataSourceName = "DatabaseDataSource", CacheDataSourceName = "ReliableStateDataSource" }
            });
            return result;
        }

        private IRepository<TEntity> GetRepository<TEntity>(IServiceProvider provider)
        {
            return (IRepository<TEntity>)provider.GetRequiredService<RepositoryFactory>().Get(typeof(TEntity));
        }

        private void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<ServiceContext>(Context)
                .AddSingleton<IReliableStateManager>(StateManager)
                .AddSingleton<IConfigurationProvider, ServiceConfigurationProvider>()
                .AddSingleton<ITransactionManager, TransactionManager>()
                .AddSingleton<RulesetScheduleProvider>()
                .AddSingleton<RulesetUnpacker>()
                .AddSingleton<RulesetUnpackingDecorator>()
                .AddSingleton<DataSourceFactory>(CreateDataSourceFactory)
                .AddSingleton<RepositoryFactory>(CreateRepositoryFactory)
                .AddTransient(GetRepository<Ruleset>)
                .AddTransient(GetRepository<VersionSchedule>);
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        IWebHost host = new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(RegisterServices)
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url)
                                    .Build();

                        _serviceProvider = host.Services;
                        return host;
                    }))
            };
        }

        private async Task PreloadRulesets()
        {
            var transactionManager = _serviceProvider.GetRequiredService<ITransactionManager>();
            var scheduleProvider = _serviceProvider.GetRequiredService<RulesetScheduleProvider>();
            var rulesetRepository = _serviceProvider.GetRequiredService<IRepository<Ruleset>>();

            using (var tx = transactionManager.CreateTransaction())
            {
                var rulesetId = await scheduleProvider.GetActiveRulesetIdAsync(tx, DateTime.UtcNow);
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
