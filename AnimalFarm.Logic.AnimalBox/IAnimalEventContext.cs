using AnimalFarm.Model;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.AnimalBox
{
    /// <summary>
    /// Describes a context of an animal event execution.
    /// </summary>
    public interface IAnimalEventContext
    {
        /// <summary>
        /// Gets or sets the target animal.
        /// </summary>
        Animal Animal { get; set; }
        
        /// <summary>
        /// Gets or sets the currently active ruleset.
        /// </summary>
        Ruleset ActiveRuleset { get; set; }

        /// <summary>
        /// Gets a ruleset by a specific version.
        /// </summary>
        Task<Ruleset> GetRulesetAsync(string rulesetId);
    }
}
