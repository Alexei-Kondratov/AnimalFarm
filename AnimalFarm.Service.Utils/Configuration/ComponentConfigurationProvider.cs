using AnimalFarm.Utils.Configuration;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Configuration
{
    public class ComponentConfigurationProvider : IComponentConfigurationProvider
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ComponentConfigurationMap _componentConfigurationMap;

        public ComponentConfigurationProvider(IConfigurationProvider configurationProvider, ComponentConfigurationMap componentConfigurationMap)
        {
            _configurationProvider = configurationProvider;
            _componentConfigurationMap = componentConfigurationMap;
        }

        public async Task<IComponentConfiguration> GetConfigurationAsync(Type componentType, string name)
        {
            Type configurationType = _componentConfigurationMap.GetConfigurationTypeFor(componentType);
            return (IComponentConfiguration) await _configurationProvider.GetConfigurationAsync(configurationType, name);
        }
    }
}
