﻿using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes a single animal.
    /// </summary>
    public class Animal : ComplexTableEntity, IHaveId<string>
    {
        private string _id;

        /// <summary>
        /// Gets or sets the entities identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                RowKey = Id;
                PartitionKey = Id;
            }
        }

        /// <summary>
        /// Gets or sets the animal's owner id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the animal's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the animal's animal type id.
        /// </summary>
        public string TypeId { get; set; }
        
        /// <summary>
        /// Get or sets the animal's current attributes.
        /// </summary>
        public Dictionary<string, decimal> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the time when the animal's data was last calculated (based on events and over-time effects.
        /// </summary>
        public DateTime LastCalculated { get; set; }

        /// <summary>
        /// Gets or sets the entity's creation timestamp.
        /// </summary>
        public DateTime Created { get; set; }

    }
}
