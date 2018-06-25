using AnimalFarm.Data;
using AnimalFarm.Data.Cache;
using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Service.Utils.Tracing;
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

namespace AnimalFarm.Service
{
    public abstract class BaseStatefulService : StatefulService
    {
        protected IServiceProvider ServiceProvider { get; set; }

        protected BaseStatefulService(StatefulServiceContext serviceContext)
            : base(serviceContext)
        {
        }

        protected BaseStatefulService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica)
            : base(serviceContext, reliableStateManagerReplica)
        {
        }

        protected virtual void SetupWebHost(IWebHostBuilder builder)
        {
        }

        protected virtual void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddAnimalFarmCommonServices()
                .AddSingleton<ServiceContext>(Context)
                .AddSingleton<IReliableStateManager>(StateManager);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

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
