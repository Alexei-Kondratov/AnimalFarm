using System;

namespace AnimalFarm.Utils.Configuration
{
    /// <summary>
    /// Base partial implementation for the IConfigurableComponentBuilder interface.
    /// </summary>
    /// <typeparam name="TConfiguration">Type of the configurations that serve as the builder's input.</typeparam>
    /// <typeparam name="TComponent">Type of components build by the builder.</typeparam>
    /// <typeparam name="TConfigurationKey">Type of the key used to identify the TConfiguration entities.</typeparam>
    public abstract class ConfigurationTypedComponentBuilder<TConfiguration, TComponent, TConfigurationKey> 
        : IConfigurableComponentBuilder<TComponent, TConfigurationKey>
        where TConfiguration : IComponentConfiguration<TConfigurationKey>
    {
        protected abstract TComponent Build(TConfiguration configuration, IServiceProvider serviceContainer);

        public TComponent Build(IComponentConfiguration<TConfigurationKey> configuration, IServiceProvider serviceContainer)
        {
            return Build((TConfiguration)configuration, serviceContainer);
        }

        public bool CanBuild(IComponentConfiguration<TConfigurationKey> configuration)
        {
            return configuration is TConfiguration;
        }
    }
}
