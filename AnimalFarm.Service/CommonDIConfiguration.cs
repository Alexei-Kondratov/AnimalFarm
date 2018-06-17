using AnimalFarm.Data;
using AnimalFarm.Data.DataSources;
using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.AspNet;
using AnimalFarm.Service.Utils.Communication;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Service.Utils.Operations;
using AnimalFarm.Service.Utils.Tracing;
using AnimalFarm.Utils.Configuration;
using AnimalFarm.Utils.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        public static IServiceCollection AddAnimalFarmCommonServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IConfigurationProvider>(CreateConfigurationProvider)
                .AddSingleton<ITransactionManager, TransactionManager>()
                .AddSingleton<IRequestContextAccessor, AspNetRequestContextAccessor>()
                .AddSingleton<ServiceLocator>()
                .AddSingleton<IServiceHttpClientFactory, ServiceHttpClientFactory>()
                .AddSingleton<OperationRunner>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<JwtManager>()
                .AddSingleton<ServiceEventSource>(ServiceEventSource.Current)
                .AddSingleton<ServiceLogger>();
        }
    }
}
