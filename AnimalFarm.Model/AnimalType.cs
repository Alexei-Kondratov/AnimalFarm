using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes a type of animals that defines baseline charateristics of an individual animal.
    /// </summary>
    public class AnimalType
    {
        /// <summary>
        /// Gets or sets the entity's identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the animal type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attribute definitions for the animal type.
        /// </summary>
        public Dictionary<string, AnimalTypeAttribute> Attributes { get; set; }
    }
}
