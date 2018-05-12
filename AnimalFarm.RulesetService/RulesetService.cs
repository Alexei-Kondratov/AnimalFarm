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
using Microsoft.ServiceFabric.Data.Collections;
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

        public RulesetService(StatefulServiceContext context, IRepository<Ruleset> rulesetRepository)
            : base(context)
        {
            _rulesetRepository = rulesetRepository;
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
                                            .AddSingleton<IReliableStateManager>(this.StateManager))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        private async Task<IReliableDictionary<string, Ruleset>> GetRulesetsAsync()
        {
            return await StateManager.GetOrAddAsync<IReliableDictionary<string, Ruleset>>("Rulesets");
        }

        private async Task<Ruleset> GetRulesetAsync(string rulesetId)
        {
            var rulesets = await GetRulesetsAsync();

            using (var tx = StateManager.CreateTransaction())
            {
                ServiceEventSource.Current.ServiceMessage(Context, $"Requesting ruleset {rulesetId}");
                var currentValue = await rulesets.TryGetValueAsync(tx, rulesetId);
                if (currentValue.HasValue)
                {
                    ServiceEventSource.Current.ServiceMessage(Context, $"Retrieved ruleset {rulesetId} from cache");
                    return currentValue.Value;
                }

                Ruleset ruleset = await _rulesetRepository.ByIdAsync(rulesetId);

                await rulesets.AddAsync(tx, ruleset.Id, ruleset);
                await tx.CommitAsync();
                ServiceEventSource.Current.ServiceMessage(Context, $"Loaded ruleset {rulesetId}");
                return ruleset;
            }
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var ruleset = await GetRulesetAsync("BaseRuleset");
            ruleset = await GetRulesetAsync("BaseRuleset");
            ruleset = await GetRulesetAsync("BaseRuleset");
            ServiceEventSource.Current.ServiceMessage(Context, $"Loaded {ruleset.Id}");
            cancellationToken.WaitHandle.WaitOne();
        }
    }
}
