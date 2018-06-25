using AnimalFarm.Utils.Configuration;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Utils.Configuration
{
    public interface IComponentConfigurationProvider
    {
        Task<IComponentConfiguration> GetConfigurationAsync(Type componentType, string name);
    }
}
