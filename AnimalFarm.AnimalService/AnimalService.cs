using AnimalFarm.Data;
using AnimalFarm.Data.DataSources;
using AnimalFarm.Data.Repositories;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.Configuration;
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
            _transactionManager = new TransactionManager();
            var configProvider = new ServiceConfigurationProvider(context);
            var dbDataSource = new DocumentDbDataSource("Database");
            var rulesetServiceProxyDataSource = new ServiceProxyDataSource("RulesetService", ServiceType.Ruleset);
            var reliableStateDataSource = new ReliableStateDataSource("ReliableState", StateManager);
            _animalRepository =
                new CachedRepository<Animal>(
                    new DataSourceRepository<Animal>
                        (reliableStateDataSource, "Animals"),                
                    new DataSourceRepository<Animal>
                        (dbDataSource, "Animals")
                        );
            _rulesetRepository =
                    new CachedRepository<Ruleset>(
                        new DataSourceRepository<Ruleset>
                            (reliableStateDataSource, "Rulesets"),
                        new DataSourceRepository<Ruleset>
                            (rulesetServiceProxyDataSource, "Rulesets")
            );
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
                await _rulesetRepository.ByIdAsync(tx, "BaseRuleset", "BaseRuleset");
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
