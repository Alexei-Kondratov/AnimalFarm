using AnimalFarm.Utils.Configuration;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Data.DataSources.Configuration
{
    public class DataSourceFactory : ConfigurableComponentFactory<IDataSource, string>
    {
        public DataSourceFactory(IServiceProvider serviceContainer, IEnumerable<IConfigurableComponentBuilder<IDataSource, string>> builders)
            : base(serviceContainer, builders)
        {
        }
    }
}
