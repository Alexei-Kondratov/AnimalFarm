using System.Collections.Generic;
using System.Fabric;
using System.IO;
using AnimalFarm.Data;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace AnimalFarm.AuthenticationService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class AuthenticationService : StatelessService
    {
        private ITransactionManager _transactionManager;
        private IRepository<UserAuthenticationInfo> _userRepository;

        public AuthenticationService(StatelessServiceContext context)
            : base(context)
        {
            _transactionManager = new StatelessServiceTransactionManager();
            var repositoryBuilder = new RepositoryBuilder(context, null);
            _userRepository = repositoryBuilder.BuildRepository<UserAuthenticationInfo>();
        }

        private void ConfigureServices(StatelessServiceContext context, IServiceCollection services)
        {
            services
                .AddSingleton<StatelessServiceContext>(context)
                .AddSingleton<ITransactionManager>(_transactionManager)
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
