using AnimalFarm.Data;
using AnimalFarm.Data.DataSources;
using AnimalFarm.Data.Repositories;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Utils.Configuration;
using AnimalFarm.Utils.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
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
        private IRepository<Ruleset> _rulesetRepository;
        private IRepository<VersionSchedule> _scheduleRepository;
        private ITransactionManager _transactionManager;

        public RulesetService(StatefulServiceContext context)
            : base(context)
        {
            _transactionManager = new TransactionManager();
            var configProvider = new ServiceConfigurationProvider(context);
            var dbDataSource = new DocumentDbDataSource("Database");
            var reliableStateDataSource = new ReliableStateDataSource("ReliableState", StateManager);
            var rawRulesetRepository = new DataSourceRepository<Ruleset, DocumentDbDataSource, DocumentDbTransactionContext>
                            (dbDataSource, "Rulesets");
            var rulesetUnpacker = new RulesetUnpacker(rawRulesetRepository);
            var rulesetUnpackingTransformation = new RulesetUnpackingTransformation(rulesetUnpacker);
            var rulesetUnpackingRepository = new TransformingRepositoryDecorator<Ruleset>(rawRulesetRepository, rulesetUnpackingTransformation);
            _rulesetRepository =
                    new CachedRepository<Ruleset>(
                        new DataSourceRepository<Ruleset, ReliableStateDataSource, ReliableStateTransactionContext>
                            (reliableStateDataSource, "Rulesets"),
                        rulesetUnpackingRepository);

            _scheduleRepository =
                new CachedRepository<VersionSchedule>(
                    new DataSourceRepository<VersionSchedule, ReliableStateDataSource, ReliableStateTransactionContext>
                        (reliableStateDataSource, "VersionSchedules"),
                    new DataSourceRepository<VersionSchedule, DocumentDbDataSource, DocumentDbTransactionContext>
                        (dbDataSource, "VersionSchedules"));
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

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatefulServiceContext>(serviceContext)
                                            .AddSingleton<IReliableStateManager>(StateManager)
                                            .AddSingleton<ITransactionManager>(_transactionManager)
                                            .AddSingleton<IRepository<Ruleset>>(_rulesetRepository)
                                            .AddSingleton(new RulesetScheduleProvider("Default", _scheduleRepository)))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        private async Task PreloadRulesets()
        {
            using (var tx = _transactionManager.CreateTransaction())
            {
                var ruleset = await _rulesetRepository.ByIdAsync(tx, "BaseRuleset", "BaseRuleset");
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
