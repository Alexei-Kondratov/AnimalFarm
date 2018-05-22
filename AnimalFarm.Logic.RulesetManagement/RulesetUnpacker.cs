using AnimalFarm.Data;
using AnimalFarm.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.RulesetManagement
{
    public class RulesetUnpacker
    {
        private readonly IRepository<Ruleset> _rulesets;

        public RulesetUnpacker(IRepository<Ruleset> rulesets)
        {
            _rulesets = rulesets;
        }

        private Dictionary<string, TValue> Merge<TValue>(Dictionary<string, TValue> @base, Dictionary<string, TValue> inherited)
        {
            var result = new Dictionary<string, TValue>(@base);
            foreach (string key in inherited.Keys)
            {
                if (result.ContainsKey(key))
                    result[key] = inherited[key];
                else
                    result.Add(key, inherited[key]);
            }

            return result;
        }

        public async Task<Ruleset> UnpackAsync(ITransaction transaction, Ruleset ruleset)
        {
            if (ruleset.InheritedRulesetId == null)
                return ruleset;

            Ruleset baseRuleset = await _rulesets.ByIdAsync(transaction, ruleset.InheritedRulesetId, ruleset.InheritedRulesetId);
            ruleset.AnimalActions = Merge(baseRuleset.AnimalActions, ruleset.AnimalActions);
            ruleset.AnimalTypes = Merge(baseRuleset.AnimalTypes, ruleset.AnimalTypes);
            return ruleset;
        }
    }
}
