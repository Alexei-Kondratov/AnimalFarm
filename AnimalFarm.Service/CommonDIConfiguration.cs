using AnimalFarm.Data;
using AnimalFarm.Data.DataSources;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Service.Utils.Communication;
using AnimalFarm.Service.Utils.Configuration;
using AnimalFarm.Service.Utils.Operations;
using AnimalFarm.Service.Utils.Tracing;
using AnimalFarm.Utils.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AnimalFarm.Service
{
    public static class CommonDIConfiguration
    {
        private static IConfigurationProvider CreateConfigurationProvider(IServiceProvider services)
        {
            DocumentDbDataSource dataSource = new DocumentDbDataSource("ConfigurationDataSource");
            return new DataSourceBackedConfigurationProvider(dataSource, services.GetRequiredService<ITransactionManager>());
        }

        public static IServiceCollection AddAnimalFarmCommonServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ServiceEventSource>(ServiceEventSource.Current)
                .AddSingleton<IConfigurationProvider>(CreateConfigurationProvider)
                .AddSingleton<ITransactionManager, TransactionManager>()
                .AddSingleton<ServiceLocator>()
                .AddSingleton<IServiceHttpClientFactory, ServiceHttpClientFactory>()
                .AddSingleton<OperationRunner>();
        }
    }
}
