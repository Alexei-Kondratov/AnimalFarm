using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalFarm.Utils.Configuration
{
    /// <summary>
    /// Responsbile for providing components based on app configuration.
    /// </summary>
    /// <typeparam name="TComponent">The type of the components built by the factory.</typeparam>
    /// <typeparam name="TConfigurationKey">The type of keys identifying configuration records.</typeparam>
    public class ConfigurableComponentFactory<TComponent, TConfigurationKey>
    {
        protected readonly object _syncObj = new object();
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IEnumerable<IConfigurableComponentBuilder<TComponent, TConfigurationKey>> _builders;
        protected readonly Dictionary<TConfigurationKey, TComponent> _dataSources = new Dictionary<TConfigurationKey, TComponent>();
        protected IEnumerable<IComponentConfiguration<TConfigurationKey>> _configurations;

        public ConfigurableComponentFactory(IServiceProvider serviceProvider, IEnumerable<IConfigurableComponentBuilder<TComponent, TConfigurationKey>> builders)
        {
            _serviceProvider = serviceProvider;
            _builders = builders;
        }

        public void SetConfigurations(IEnumerable<IComponentConfiguration<TConfigurationKey>> configurations)
        {
            _configurations = configurations;
        }

        public IEnumerable<IComponentConfiguration<TConfigurationKey>> Configurations => _configurations;

        public TComponent Get(TConfigurationKey key)
        {
            lock (_syncObj)
            {
                if (_dataSources.TryGetValue(key, out TComponent existingComponent))
                    return existingComponent;

                IComponentConfiguration<TConfigurationKey> configuration = _configurations.First(c => c.Key.Equals(key));
                IConfigurableComponentBuilder<TComponent, TConfigurationKey> builder = _builders.First(b => b.CanBuild(configuration));
                var dataSource = builder.Build(configuration, _serviceProvider);
                _dataSources.Add(key, dataSource);
                return dataSource;
            }
        }
    }
}
