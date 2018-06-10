using System.Threading.Tasks;

namespace AnimalFarm.Utils.Configuration
{
    public interface IConfigurationProvider
    {
        Task<TConfiguration> GetConfigurationAsync<TConfiguration>(string configurationName = null);
    }
}
