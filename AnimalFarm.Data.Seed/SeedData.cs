using AnimalFarm.Model;
using AnimalFarm.Utils.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalFarm.Data.Seed
{
    public class SeedData : ISeedData
    {
        public IEnumerable<SeedCollection> Collections { get; private set; }

        public SeedData()
        {
            Collections = new[] {
                new SeedCollection {
                    Name = "Rulesets",
                    EntityType = typeof(Ruleset),
                    Entities = GetRulesets()
                },
                new SeedCollection {
                    Name = "Animals",
                    EntityType = typeof(Animal),
                    Entities = new Animal[] { }
                },
                new SeedCollection {
                    Name = "UserAuthenticationInfos",
                    EntityType = typeof(UserAuthenticationInfo),
                    Entities = GetUserAuthenticationInfos()
                },
                new SeedCollection {
                    Name = "VersionSchedules",
                    EntityType = typeof(VersionSchedule),
                    Entities = GetVersionSchedules()
                }
            };
        }

        private const string _pigId = "Pig";
        private const string _horseId = "Horse";

        private const string _hungerAttributeId = "Hunger";
        private const string _happinessAttributeId = "Happiness";
        private const string _thirstAttributeId = "Thirst";

        private const string _firstVersionId = "0.1";
        private const string _secondVersionId = "0.2";
        private const string _thirdVersionId = "0.2";
        private const string _firstRulesetVersionId = "4d46fc8100634609af0e2017cae592d9";
        private const string _secondRulesetVersionId = "be602401b81d4aecb53cf62d70d1817f";
        private const string _thirdRulesetVersionId = "d5062a4ceca5495f9c783ee394265a59";

        internal const string DefaultBranchId = "30cd45c4860f481ea13bf0ea4284ea8a";

        private IEnumerable<UserAuthenticationInfo> GetUserAuthenticationInfos()
        {
            var hasher = new PasswordHasher();

            return new[]
            {
                new UserAuthenticationInfo
                {
                    Id = "FirstUserId",
                    Login = "FirstUser",
                    PasswordSalt = "SaltySalt",
                    PasswordHash = hasher.GetHash("SaltySalt", "FirstPassword")
                },
                new UserAuthenticationInfo
                {
                    Id = "SecondUserId",
                    Login = "SecondUser",
                    PasswordSalt = "Sugar",
                    PasswordHash = hasher.GetHash("Sugar", "SecondPassword")
                },
            };
        }

        private IEnumerable<Ruleset> GetRulesets()
        {
            return new[]
            {
                new Ruleset
                {
                    Id = _firstRulesetVersionId,
                    Name = "Base Ruleset",
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
                    Name = "Tiger Update",
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
                },
                new Ruleset
                {
                    Id = _thirdRulesetVersionId,
                    Name = "Thirst Update",
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
                        },
                        new AnimalAction
                        {
                            Id = "Water",
                            Name = "Water",
                            AttributeEffects = new Dictionary<string, decimal>
                            {
                                { _thirstAttributeId, -20 }
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
                                },
                                {
                                    _thirstAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 20,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = 2,
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
                                },
                                {
                                    _thirstAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 30,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = 2,
                                    }
                                }
                            }
                        },
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
                                },
                                {
                                    _thirstAttributeId,
                                    new AnimalTypeAttribute
                                    {
                                         InitialValue = 20,
                                         MaxValue = 100,
                                         MinValue = 0,
                                         RatioPerMinute = 2,
                                    }
                                }
                            }
                        }
                    }.ToDictionary(a => a.Id)
                },
            };
        }

        private IEnumerable<VersionSchedule> GetVersionSchedules()
        {
            return new[]
            {

                new VersionSchedule
                {
                    BranchId = "30cd45c4860f481ea13bf0ea4284ea8a",
                    Records = new []
                    {
                        new VersionScheduleRecord { VersionId = _firstVersionId, RulesetId = _firstRulesetVersionId, Start = new DateTime(2018, 5, 1)},
                        new VersionScheduleRecord { VersionId = _secondVersionId, RulesetId = _secondRulesetVersionId, Start = DateTime.UtcNow.AddMinutes(5)},
                        new VersionScheduleRecord { VersionId = _thirdVersionId, RulesetId = _thirdRulesetVersionId, Start = DateTime.UtcNow.AddMinutes(10)},
                    }
                }
            };
        }
    }
}