using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using AnimalFarm.Model;
using AnimalFarm.Data;

namespace AnimalFarm.RulesetService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class RulesetService : StatefulService
    {
        private IRepository<Ruleset> _rulesetRepository;
        private ITransactionManager _transactionManager;

        public RulesetService(StatefulServiceContext context)
            : base(context)
        {
            _rulesetRepository = BuildRulesetRepository();
            _transactionManager = new UnifiedTransactionManager(StateManager);
        }

        private IRepository<Ruleset> BuildRulesetRepository()
        {
            var sourceRepository = new AzureTableRepository<Ruleset>
            (
                "DefaultEndpointsProtocol=https;AccountName=animalfarm;AccountKey=7Lrjq5wId8TCpSx5o7vFI4nxVugkhjZcOh25RCSp318HIeXDE4o8SkaoVgeb5vKnNtrGXkJapS+Mmuf0Tnp7GA==;EndpointSuffix=core.windows.net",
                "Rulesets"
            );

            var cacheRepository = new ReliableStateRepository<Ruleset>(StateManager);
            return new CachedRepository<Ruleset>(cacheRepository, sourceRepository);
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
                                            .AddSingleton<IRepository<Ruleset>>(_rulesetRepository))
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
            await PreloadRulesets();
            cancellationToken.WaitHandle.WaitOne();
        }
    }
}
