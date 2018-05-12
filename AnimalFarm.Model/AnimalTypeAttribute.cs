namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes an animal attribute properties defined by a specific animal type.
    /// </summary>
    public class AnimalTypeAttribute
    {
        /// <summary>
        /// Gets or sets the minimum value of the attribute.
        /// </summary>
        public decimal MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the attribute.
        /// </summary>
        public decimal MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the value by which the attribute changed each minute.
        /// </summary>
        public decimal RatioPerMinute { get; set; }

        /// <summary>
        /// Gets or sets the initial value of the attribute for new animals.
        /// </summary>
        public decimal InitialValue { get; set; }
    }
}
