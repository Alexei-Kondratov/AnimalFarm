using System;

namespace AnimalFarm.Utils.Configuration
{
    /// <summary>
    /// Responsible for materializing app components from a configuration record.
    /// </summary>
    /// <typeparam name="TComponent">The type of the components which are served by the builder.</typeparam>
    /// <typeparam name="TKey">The type of the configuration key.</typeparam>
    public interface IConfigurableComponentBuilder
    {
        /// <summary>
        /// Returns a value indicating whether the builder can process the configuration record.
        /// </summary>
        /// <param name="configuration">A component configuration.</param>
        bool CanBuild(IComponentConfiguration configuration);

        /// <summary>
        /// Materializes an app component from the configuration record. 
        /// </summary>
        /// <param name="configuration">Component configuration.</param>
        /// <param name="serviceContainer">Depnendency injection container.</param>
        object Build(IComponentConfiguration configuration, IServiceProvider serviceContainer);
    }
}
