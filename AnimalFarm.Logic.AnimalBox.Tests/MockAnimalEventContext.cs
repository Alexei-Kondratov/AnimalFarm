using System.Linq;
using System.Collections.Generic;
using AnimalFarm.Model;

namespace AnimalFarm.Logic.AnimalBox.Tests
{
    /// <summary>
    /// Implements IAnimalEventContext for tests.
    /// </summary>
    public class MockAnimalEventContext : IAnimalEventContext
    {

        public Animal Animal { get ; set; }
        public Ruleset ActiveRuleset { get; set; }
        public Dictionary<string, Ruleset> Rulesets { get; set; }

        public Ruleset GetRuleset(string rulesetVersionId)
        {
            return Rulesets[rulesetVersionId];
        }

        public MockAnimalEventContext(Animal animal, params Ruleset[] rulesets)
        {
            Animal = animal;
            ActiveRuleset = rulesets[0];

            if (ActiveRuleset.Id != null)
                Rulesets = rulesets.ToDictionary(r => r.Id);
        }

    }
}
