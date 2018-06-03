using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalFarm.Model.Events
{
    /// <summary>
    /// The base class for describing an event that changes an animal's state.
    /// </summary>
    public abstract class AnimalEvent
    {
        public abstract string EventType { get; }

        /// <summary>
        /// Gets or sets the event's unique identifier.
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the last event executed for the animal.
        /// </summary>
        public string PreviousEventId { get; set; }

        /// <summary>
        /// Gets or sets the time of the event.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the id of the user that initiated the event. Null if the event is system-initiated.
        /// </summary>
        public string ActingUserId { get; set; }

        /// <summary>
        /// Gets or sets the id of the user that owns the animal.
        /// </summary>
        public string OwnerUserId { get; set; }

        /// <summary>
        /// Gets or sets the animal's id.
        /// </summary>
        public string AnimalId { get; set; }
    }
}
