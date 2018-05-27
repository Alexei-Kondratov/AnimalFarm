
using System;

namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes a past or planned version change.
    /// </summary>
    public class VersionScheduleRecord
    {
        /// <summary>
        /// Gets or sets the id of the version.
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// Gets or sets the id of the active ruleset for the version.
        /// </summary>
        public string RulesetId { get; set; }

        /// <summary>
        /// Gets or sets the scheduled time of the version change.
        /// </summary>
        public DateTime Start { get; set; }
    }
}
