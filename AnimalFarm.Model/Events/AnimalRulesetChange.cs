
namespace AnimalFarm.Model.Events
{
    /// <summary>
    /// Represents an update of the active ruleset. 
    /// </summary>
    public class AnimalRulesetChangeEvent : AnimalEvent
    {
        /// <summary>
        /// Gets or sets the id of the version associated with the new ruleset.
        /// </summary>
        public string NewVersionId { get; set; }
    }
}
