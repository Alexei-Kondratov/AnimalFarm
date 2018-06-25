
namespace AnimalFarm.Utils.Configuration
{
    /// <summary>
    /// Base interface for a component configuration.
    /// </summary>
    /// <typeparam name="TKey">Type of the key used to identify the configuration.</typeparam>
    public interface IComponentConfiguration
    {
        /// <summary>
        /// Gets the key used to identify the configuration.
        /// </summary>
        string Key { get; }
    }
}
