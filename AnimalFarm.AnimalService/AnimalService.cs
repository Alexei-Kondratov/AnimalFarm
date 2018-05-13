using AnimalFarm.Data;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils;
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

namespace AnimalFarm.AnimalService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class AnimalService : StatefulService
    {
        private ITransactionManager _transactionManager;
        private IRepository<Animal> _animalRepository;
        private IRepository<Ruleset> _rulesetRepository;

        public AnimalService(StatefulServiceContext context)
            : base(context)
        {
            _transactionManager = new UnifiedTransactionManager(StateManager);
            _animalRepository = BuildAnimalRepository();
            _rulesetRepository = BuildRulesetRepository();
        }

        private IRepository<Animal> BuildAnimalRepository()
        {
            var sourceRepository = new AzureTableRepository<Animal>
            (
                "DefaultEndpointsProtocol=https;AccountName=animalfarm;AccountKey=7Lrjq5wId8TCpSx5o7vFI4nxVugkhjZcOh25RCSp318HIeXDE4o8SkaoVgeb5vKnNtrGXkJapS+Mmuf0Tnp7GA==;EndpointSuffix=core.windows.net",
                "Animals"
            );

            var cacheRepository = new ReliableStateRepository<Animal>(StateManager);
            return new CachedRepository<Animal>(cacheRepository, sourceRepository);
        }

        private IRepository<Ruleset> BuildRulesetRepository()
        {
            var sourceRepository = new ReadOnlyProxyRepository<Ruleset>(ServiceType.Ruleset, "");
            return sourceRepository;
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
                                            .AddSingleton<IReliableStateManager>(this.StateManager)
                                            .AddSingleton<ITransactionManager>(_transactionManager)
                                            .AddSingleton<IRepository<Animal>>(_animalRepository)
                                            .AddSingleton<IRepository<Ruleset>>(_rulesetRepository))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        private async Task PreloadRuleset()
        {
            using (var tx = _transactionManager.CreateTransaction())
            {
                await _rulesetRepository.ByIdAsync(tx, "", "");
                await tx.CommitAsync();
            }
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await PreloadRuleset();
            cancellationToken.WaitHandle.WaitOne();
        }
    }
}
