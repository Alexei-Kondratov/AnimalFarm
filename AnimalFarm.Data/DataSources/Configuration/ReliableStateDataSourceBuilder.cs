using AnimalFarm.Utils.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using System;

namespace AnimalFarm.Data.DataSources.Configuration
{
    public class ReliableStateDataSourceBuilder : ConfigurationTypedComponentBuilder<ReliableStateDataSourceConfiguration, IDataSource, string>
    {
        protected override IDataSource Build(ReliableStateDataSourceConfiguration configuration, IServiceProvider serviceContainer)
        {
            return new ReliableStateDataSource(configuration.Key, serviceContainer.GetRequiredService<IReliableStateManager>());
        }
    }
}
