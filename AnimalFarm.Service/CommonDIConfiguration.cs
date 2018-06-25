using AnimalFarm.Data;
using AnimalFarm.Data.DataSources;
using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Repositories.Configuration;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.AspNet;
using AnimalFarm.Service.Utils.Communication;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Service.Utils.Operations;
using AnimalFarm.Service.Utils.Tracing;
using AnimalFarm.Utils.Configuration;
using AnimalFarm.Utils.DependencyInjection;
using AnimalFarm.Utils.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Fabric;

namespace AnimalFarm.Service
{
    public static class CommonDIConfiguration
    {
        private static IConfigurationProvider CreateConfigurationProvider(IServiceProvider services)
        {
            var serviceContext = services.GetRequiredService<ServiceContext>();
            var configSection = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["ConfigurationConnection"];
            var connectionInfo = new DocumentDbConnectionInfo
            {
                DatabaseName = configSection.Parameters["DatabaseName"].Value,
                Key = configSection.Parameters["Key"].Value,
                Uri = new Uri(configSection.Parameters["Uri"].Value)
            };

            var dataSource = new DocumentDbDataSource("ConfigurationDataSource", connectionInfo);
            return new DataSourceBackedConfigurationProvider(dataSource, services.GetRequiredService<ITransactionManager>());
        }

        private static ComponentConfigurationMap CreateComponentConfigurationMap(IServiceProvider services)
        {
            return new ComponentConfigurationMap
            (
                new Dictionary<Type, Type>
                {
                    { typeof(IDataSource), typeof(DataSourceConfiguration) },
                    { typeof(IRepository<>), typeof(RepositoryConfiguration) }
                }
            );
        }

        private static ConfigurableComponentsFactory CreateConfigurableComponentsFactory(IServiceProvider serviceProvider)
        {
            return new ConfigurableComponentsFactory(serviceProvider,
                new Dictionary<Type, IEnumerable<IConfigurableComponentBuilder>>
                {
                    {
                        typeof(IDataSource),
                        new IConfigurableComponentBuilder[] {
                            new ReliableStateDataSourceBuilder(),
                            new DocumentDbDataSourceBuilder(),
                            new ServiceProxyDataSourceBuilder(),
                        }
                    },
                    {
                        typeof(IRepository<>),
                        new IConfigurableComponentBuilder [] {
                            new DataSourceRepositoryBuilder()
                        }
                    }
                });        
        }

        public static IServiceCollection AddAnimalFarmCommonServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IConfigurationProvider>(CreateConfigurationProvider)
                .AddSingleton<ComponentConfigurationMap>(CreateComponentConfigurationMap)
                .AddSingleton<ConfigurableComponentsFactory>(CreateConfigurableComponentsFactory)
                .AddSingleton<IComponentConfigurationProvider, ComponentConfigurationProvider>()
                .AddSingleton<INamedServiceProvider, ConfigurableNamedServiceProvider>()
                .AddSingleton<ITransactionManager, TransactionManager>()
                .AddSingleton<IRequestContextAccessor, AspNetRequestContextAccessor>()
                .AddSingleton<ServiceLocator>()
                .AddSingleton<IServiceHttpClientFactory, ServiceHttpClientFactory>()
                .AddSingleton<OperationRunner>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<JwtManager>()
                .AddSingleton<ServiceEventSource>(ServiceEventSource.Current)
                .AddSingleton<ILogger, ServiceLogger>();
        }
    }
}
