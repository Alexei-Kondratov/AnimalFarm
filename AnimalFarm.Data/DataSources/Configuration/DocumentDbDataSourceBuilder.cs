using AnimalFarm.Utils.Configuration;
using AnimalFarm.Utils.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalFarm.Data.DataSources.Configuration
{
    public abstract class DataSourceBuilder<TConfiguration> : ConfigurationTypedComponentBuilder<TConfiguration, IDataSource, string>
        where TConfiguration : DataSourceConfiguration
    {
        protected abstract IDataSource BuildBase(TConfiguration configuration, IServiceProvider serviceContainer);

        private IDataSource AddDecorator(IDataSource dataSource, Type decoratorType, IServiceProvider serviceProvider)
        {
            return (IDataSource)ResolveDependenciesHelper.Instantiate(decoratorType, serviceProvider, dataSource);
        }

        protected sealed override IDataSource Build(TConfiguration configuration, IServiceProvider serviceContainer)
        {
            var result = BuildBase(configuration, serviceContainer);
            if (configuration.Decorators?.Any() == true)
            {
                foreach (Type decoratorType in configuration.Decorators)
                    result = AddDecorator(result, decoratorType, serviceContainer);
            }

            return result;
        }
    }

    public class DocumentDbDataSourceBuilder : DataSourceBuilder<DocumentDbDataSourceConfiguration>
    {
        protected override IDataSource BuildBase(DocumentDbDataSourceConfiguration configuration, IServiceProvider serviceContainer)
        {
            var configProvier = serviceContainer.GetService<IConfigurationProvider>();
            var connectionInfo = configProvier.GetConfigurationAsync<DocumentDbConnectionInfo>(configuration.ConnectionInfoName).GetAwaiter().GetResult();

            return new DocumentDbDataSource(configuration.Key, connectionInfo);
        }
    }
}
