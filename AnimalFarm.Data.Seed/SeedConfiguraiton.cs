using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Service.Utils.Configuration;
using System.Collections.Generic;

namespace AnimalFarm.Data.Seed
{
    public class SeedConfiguraiton : ISeedData
    {
        public IEnumerable<SeedCollection> Collections { get; }

        public SeedConfiguraiton()
        {
            Collections = new[] {
                new SeedCollection
                {
                    Name = "Configuration",
                    Entities = new [] {
                        new ConfigurationRecord("", new BranchConfiguration { ActiveBranchId = SeedData.DefaultBranchId })
                    }
                }
            };
        }
    }
}
