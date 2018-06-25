
using AnimalFarm.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalFarm.Utils.DependencyInjection
{
    /// <summary>
    /// Responsible for intantiating component based on configuration.
    /// </summary>
    public class ConfigurableComponentsFactory
    {
        private IServiceProvider _serviceProvider;
        private Dictionary<Type, IEnumerable<IConfigurableComponentBuilder>> _builders;

        public ConfigurableComponentsFactory(IServiceProvider serviceProvider, Dictionary<Type, IEnumerable<IConfigurableComponentBuilder>> builders)
        {
            _serviceProvider = serviceProvider;
            _builders = builders;
        }

        public object InstantiateComponent(Type type, IComponentConfiguration configuration)
        {
            if (!_builders.ContainsKey(type))
                return null;

            IConfigurableComponentBuilder builder = _builders[type].First(b => b.CanBuild(configuration));
            return builder.Build(configuration, _serviceProvider);
        }
    }
}
