using System.Collections.Generic;

namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes a player action that can be applied to an animal.
    /// </summary>
    public class AnimalAction
    {
        /// <summary>
        /// Gets the entity's identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the action's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets modifiers which describe how the action changes an animal's attributes.
        /// </summary>
        public Dictionary<string, decimal> AttributeEffects { get; set; }
    }
}
