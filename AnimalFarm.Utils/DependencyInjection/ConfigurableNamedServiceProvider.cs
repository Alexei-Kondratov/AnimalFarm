using AnimalFarm.Utils.Configuration;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalFarm.Utils.DependencyInjection
{
    /// <summary>
    /// INamedServiceProvider implementation for configurable components.
    /// </summary>
    public class ConfigurableNamedServiceProvider : INamedServiceProvider
    {
        private IComponentConfigurationProvider _componentConfigurationProvider;
        private ConfigurableComponentsFactory _configurableComponentsFactory;

        private ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> _componentInstances 
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();

        public ConfigurableNamedServiceProvider(IComponentConfigurationProvider componentConfigurationProvider, ConfigurableComponentsFactory configurableComponentsFactory)
        {
            _componentConfigurationProvider = componentConfigurationProvider;
            _configurableComponentsFactory = configurableComponentsFactory;
        }

        private async Task<object> InstantiateComponentAsync(Type type, string name)
        {
            IComponentConfiguration configuration = await _componentConfigurationProvider.GetConfigurationAsync(type, name);
            return _configurableComponentsFactory.InstantiateComponent(type, configuration);

        }

        public async Task<TService> GetServiceAsync<TService>(string name)
        {
            var instances = _componentInstances.GetOrAdd(typeof(TService), new ConcurrentDictionary<string, object>());
            var instance = instances.GetOrAdd(name, await InstantiateComponentAsync(typeof(TService), name));
            return (TService)instance;
        }

        public IEnumerable<TType> GetAllServices<TType>()
        {
            return _componentInstances.Values.SelectMany(cs => cs.Values).OfType<TType>().ToList();
        }
    }
}
