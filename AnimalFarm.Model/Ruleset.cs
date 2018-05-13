using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes a set of rules.
    /// </summary>
    public class Ruleset : TableEntity, IHaveId<string>
    {
        private string _id;

        /// <summary>
        /// Gets or sets the entities identifier.
        /// </summary>
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
        public string Name { get; set; }
        public string InheritedRulesetId { get; set; }

        public Dictionary<string, AnimalType> AnimalTypes { get; set; }
        public Dictionary<string, AnimalAction> AnimalActions { get; set; }
    }
}
