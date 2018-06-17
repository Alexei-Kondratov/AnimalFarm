using AnimalFarm.Data;
using AnimalFarm.Data.DataSources;
using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories;
using AnimalFarm.Model;
using AnimalFarm.Service;
using AnimalFarm.Service.Utils.Tracing;
using AnimalFarm.Utils.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;

namespace AnimalFarm.AuthenticationService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class AuthenticationService : StatelessService
    {
        private IRepository<UserAuthenticationInfo> _userRepository;

        public AuthenticationService(StatelessServiceContext context)
            : base(context)
        {
            var configSection = context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["ConfigurationConnection"];
            var connectionInfo = new DocumentDbConnectionInfo
            {
                DatabaseName = configSection.Parameters["DatabaseName"].Value,
                Key = configSection.Parameters["Key"].Value,
                Uri = new Uri(configSection.Parameters["Uri"].Value)
            };

            var dbDataSource = new DocumentDbDataSource("Database", connectionInfo);
            _userRepository = new DataSourceRepository<UserAuthenticationInfo>
                (dbDataSource, "UserAuthenticationInfos");
        }

        private void ConfigureServices(StatelessServiceContext context, IServiceCollection services)
        {
            services
                .AddAnimalFarmCommonServices()
                .AddSingleton<ServiceContext>(context)
                .AddSingleton<IRepository<UserAuthenticationInfo>>(_userRepository)
                .AddSingleton<PasswordHasher>(new PasswordHasher())
                .AddSingleton<JwtManager>(new JwtManager());
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(services => ConfigureServices(serviceContext, services))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
}
