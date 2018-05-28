using AnimalFarm.Utils.Configuration;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Data.Repositories.Configuration
{
    public class RepositoryFactory : ConfigurableComponentFactory<object, Type>
    {
        public RepositoryFactory(IServiceProvider serviceProvider, IEnumerable<IConfigurableComponentBuilder<object, Type>> builders) : base(serviceProvider, builders)
        {
        }
    }
}
