using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace AnimalFarm.Model
{
    public class Ruleset : TableEntity, IHaveId<string>
    {
        public Ruleset()
        {
            PartitionKey = "Rules";
        }

        private string _id;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                RowKey = Id;
            }
        }
        public string Name { get; set; }
        public string InheritedRulesetId { get; set; }

        public Dictionary<string, AnimalType> AnimalTypes { get; set; }
        public Dictionary<string, AnimalAction> AnimalActions { get; set; }
    }
}
