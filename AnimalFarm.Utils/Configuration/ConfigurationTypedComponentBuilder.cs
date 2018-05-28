using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalFarm.Utils.Configuration
{
    public abstract class ConfigurationTypedComponentBuilder<TConfiguration, TComponent, TKey> : IConfigurableComponentBuilder<TComponent, TKey>
        where TConfiguration : IComponentConfiguration<TKey>
    {
        protected abstract TComponent Build(TConfiguration configuration, IServiceProvider serviceContainer);

        public TComponent Build(IComponentConfiguration<TKey> configuration, IServiceProvider serviceContainer)
        {
            return Build((TConfiguration)configuration, serviceContainer);
        }

        public bool CanBuild(IComponentConfiguration<TKey> configuration)
        {
            return configuration is TConfiguration;
        }
    }
}
