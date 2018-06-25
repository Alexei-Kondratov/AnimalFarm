using System;
using System.Collections.Generic;

namespace AnimalFarm.Service.Utils.Configuration
{
    public class ComponentConfigurationMap
    {
        private readonly Dictionary<Type, Type> _map;

        public ComponentConfigurationMap(Dictionary<Type, Type> map)
        {
            _map = map;
        }

        public Type GetConfigurationTypeFor(Type componentType)
        {
            if (_map.ContainsKey(componentType))
                return _map[componentType];

            if (componentType.BaseType == null)
                return null;

            return _map[componentType.BaseType];
        }
    }
}
