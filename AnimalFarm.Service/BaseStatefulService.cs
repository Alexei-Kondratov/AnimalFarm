using AnimalFarm.Data;
using AnimalFarm.Data.Cache;
using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Utils.Configuration;
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
using System.Linq;

namespace AnimalFarm.Service
{
    public abstract class BaseStatefulService : StatefulService
    {
        protected IServiceProvider ServiceProvider { get; set; }
        protected ServiceEventSource EventSource { get; set; }

        protected BaseStatefulService(StatefulServiceContext serviceContext)
            : base(serviceContext)
        {
        }

        protected BaseStatefulService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica)
            : base(serviceContext, reliableStateManagerReplica)
        {
        }

        protected virtual IEnumerable<DataSourceConfiguration> GetDataSourceConfigurations()
        {
            return Enumerable.Empty<DataSourceConfiguration>();
        }

        protected virtual IEnumerable<RepositoryConfiguration> GetRepositoryConfigurations()
        {
            return Enumerable.Empty<RepositoryConfiguration>();
        }

        protected virtual void SetupWebHost(IWebHostBuilder builder)
        {

        }

        private DataSourceFactory CreateDataSourceFactory(IServiceProvider serviceProvider)
        {
            var result = new DataSourceFactory(ServiceProvider, new IConfigurableComponentBuilder<IDataSource, string>[]
                {
                    new ReliableStateDataSourceBuilder(),
                    new DocumentDbDataSourceBuilder(),
                    new ServiceProxyDataSourceBuilder()
                });
            result.SetConfigurations(GetDataSourceConfigurations());
            return result;
        }

        private RepositoryFactory CreateRepositoryFactory(IServiceProvider serviceProvider)
        {
            var result = new RepositoryFactory(ServiceProvider, new IConfigurableComponentBuilder<object, Type>[] { new DataSourceRepositoryBuilder() });

            result.SetConfigurations(GetRepositoryConfigurations());
            return result;
        }

        protected virtual void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<ServiceContext>(Context)
                .AddSingleton<IReliableStateManager>(StateManager)
                .AddSingleton<IConfigurationProvider, ServiceConfigurationProvider>()
                .AddSingleton<ITransactionManager, TransactionManager>()
                .AddSingleton<DataSourceFactory>(CreateDataSourceFactory)
                .AddSingleton<RepositoryFactory>(CreateRepositoryFactory)
                .AddSingleton<CacheManager>();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, (url, listener) =>
                    {
                        EventSource.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder =  new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(RegisterServices)
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url);

                        SetupWebHost(builder);

                        IWebHost host = builder.Build();
                        ServiceProvider = host.Services;
                        return host;
                    }))
            };
        }
    }
}
