using AnimalFarm.Model;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.AnimalBox
{
    public class AnimalBoxEventContext : IAnimalEventContext
    {
        private readonly AnimalBox _box;
        private readonly Func<string, Task<Ruleset>> _rulesetGetter;

        public Animal Animal
        {
            get => _box.Animal;
            set => _box.Animal = value;
        }

        public Ruleset ActiveRuleset
        {
            get => _box.ActiveRuleset;
            set => _box.ActiveRuleset = value;
        }

        public AnimalBoxEventContext(AnimalBox box, Func<string, Task<Ruleset>> rulesetGetter)
        {
            _box = box;
            _rulesetGetter = rulesetGetter;
        }

        public async Task<Ruleset> GetRulesetAsync(string rulesetId)
        {
            return await _rulesetGetter(rulesetId);
        }
    }
}