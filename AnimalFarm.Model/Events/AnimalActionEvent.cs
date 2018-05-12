using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalFarm.Model.Events
{
    /// <summary>
    /// Represents an action with an animal like feeding ot petting.
    /// </summary>
    public class AnimalActionEvent : AnimalEvent
    {
        /// <summary>
        /// Gets or sets the id of the type of the action.
        /// </summary>
        public string AnimalActionId { get; set; }
    }
}
