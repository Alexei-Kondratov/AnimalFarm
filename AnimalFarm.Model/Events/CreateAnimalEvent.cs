
namespace AnimalFarm.Model.Events
{
    /// <summary>
    /// Represents a creation of a new animal.
    /// </summary>
    public class CreateAnimalEvent : AnimalEvent
    {
        /// <summary>
        /// Gets or sets the name of the new animal.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the new animal's type.
        /// </summary>
        public string AnimalTypeId { get; set; }

        public override string EventType => "Create";
    }
}
