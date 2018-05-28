using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalFarm.Utils.Configuration
{
    public class ConfigurableComponentFactory<TComponent, TKey>
    {
        protected readonly object _syncObj = new object();
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IEnumerable<IConfigurableComponentBuilder<TComponent, TKey>> _builders;
        protected readonly Dictionary<TKey, TComponent> _dataSources = new Dictionary<TKey, TComponent>();
        protected IEnumerable<IComponentConfiguration<TKey>> _configurations;

        public ConfigurableComponentFactory(IServiceProvider serviceProvider, IEnumerable<IConfigurableComponentBuilder<TComponent, TKey>> builders)
        {
            _serviceProvider = serviceProvider;
            _builders = builders;
        }

        public void SetConfigurations(IEnumerable<IComponentConfiguration<TKey>> configurations)
        {
            _configurations = configurations;
        }

        public IEnumerable<IComponentConfiguration<TKey>> Configurations => _configurations;

        public TComponent Get(TKey key)
        {
            lock (_syncObj)
            {
                if (_dataSources.TryGetValue(key, out TComponent existingComponent))
                    return existingComponent;

                IComponentConfiguration<TKey> configuration = _configurations.First(c => c.Key.Equals(key));
                IConfigurableComponentBuilder<TComponent, TKey> builder = _builders.First(b => b.CanBuild(configuration));
                var dataSource = builder.Build(configuration, _serviceProvider);
                _dataSources.Add(key, dataSource);
                return dataSource;
            }
        }
    }
}
