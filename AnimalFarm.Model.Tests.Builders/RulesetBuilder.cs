using System;
using System.Collections.Generic;

namespace AnimalFarm.Model.Tests.Builders
{
    public class RulesetBuilder
    {
        private Ruleset _ruleset;

        public RulesetBuilder(string id = null)
        {
            _ruleset = new Ruleset
            {
                Id = id,
                AnimalActions = new Dictionary<string, AnimalAction>(),
                AnimalTypes = new Dictionary<string, AnimalType>()
            };
        }

        public AnimalActionBuilder WithAnimalAction(string id)
        {
            var newAnimalAction = new AnimalAction { Id = id };
            _ruleset.AnimalActions.Add(id, newAnimalAction);
            return new AnimalActionBuilder(newAnimalAction, this);
        }

        public RulesetBuilder Inheriting(string baseId)
        {
            _ruleset.InheritedRulesetId = baseId;
            return this;
        }

        public AnimalTypeBuilder WithAnimalType(string id)
        {
            var newAnimalType = new AnimalType { Id = id };
            _ruleset.AnimalTypes.Add(id, newAnimalType);
            return new AnimalTypeBuilder(newAnimalType, this);
        }

        public Ruleset Finish { get => (Ruleset)this; }

        public static implicit operator Ruleset(RulesetBuilder builder)
        {
            return builder._ruleset;
        }
    }
}
