using System;
using System.Threading.Tasks;

namespace AnimalFarm.Utils.Configuration
{
    /// <summary>
    /// Contract for app configuration sources.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Retrivies the specified configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The configuration object type.</typeparam>
        /// <param name="configurationName">The configuration name.</param>
        Task<TConfiguration> GetConfigurationAsync<TConfiguration>(string configurationName = null);

        /// <summary>
        /// Retrivies the specified configuration.
        /// </summary>
        /// <param name="type">The configuration object type.</typeparam>
        /// <param name="configurationName">The configuration name.</param>
        Task<object> GetConfigurationAsync(Type type, string configurationName = null);
    }
}
