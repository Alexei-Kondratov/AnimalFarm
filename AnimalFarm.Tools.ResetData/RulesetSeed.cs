using AnimalFarm.Model;
using System.Collections.Generic;
using System.Linq;

namespace AnimalFarm.Tools.ResetData
{
    public class RulesetSeed
    {
        private const string _pigId = "Pig";
        private const string _horseId = "Horse";

        private const string _hungerAttributeId = "Hunger";
        private const string _happinessAttributeId = "Happiness";

        private const string _firstVersionId = "0.1";
        private const string _firstRulesetVersionId = "BaseRuleset";
        private const string _secondRulesetVersionId = "TigerUpdate";

        public IEnumerable<Ruleset> GetRulesets()
        {
            return new[]
            {
                new Ruleset
                {
                    Id = _firstRulesetVersionId,
                    AnimalActions = new []
                    {
                        new AnimalAction
                        {
                            Id = "Feed",
                            Name = "Feed",
                            AttributeEffects = new Dictionary<string, decimal>
                            {
                                { _hungerAttributeId, -20 }
                            }
                        },
                        new AnimalAction
                        {
                            Id = "Pet",
                            Name = "Pet",
                            AttributeEffects = new Dictionary<string, decimal>
                            {
                                { _happinessAttributeId, 15 }
                            }
                        }
                    }.ToDictionary(a => a.Id),
                    AnimalTypes = new []
                    {
                        new AnimalType
                        {
                            Id = _horseId,
                            Name = "Horse",
                            Attributes = new Dictionary<string, AnimalTypeAttribute>
                            {
                                {
                                    _happinessAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 20,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = -2,
                                    }
                                },
                                {
                                    _hungerAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 30,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = 1,
                                    }
                                }
                            }
                        },
                        new AnimalType
                        {
                            Id = _pigId,
                            Name = "Pig",
                            Attributes = new Dictionary<string, AnimalTypeAttribute>
                            {
                                {
                                    _happinessAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 60,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = -1,
                                    }
                                },
                                {
                                    _hungerAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 50,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = 2,
                                    }
                                }
                            }
                        },
                    }.ToDictionary(a => a.Id)
                },
                new Ruleset
                {
                    Id = _secondRulesetVersionId,
                    InheritedRulesetId = _firstRulesetVersionId,
                    AnimalTypes = new []
                    {
                        new AnimalType
                        {
                            Id = "Tiger",
                            Name = "Tiger",
                            Attributes = new Dictionary<string, AnimalTypeAttribute>
                            {
                                {
                                    _happinessAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 80,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = -2,
                                    }
                                },
                                {
                                    _hungerAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 20,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = 5,
                                    }
                                }
                            }
                        }
                    }.ToDictionary(a => a.Id),
                    AnimalActions = new Dictionary<string, AnimalAction>()
                }
            };
        }
    }
}
