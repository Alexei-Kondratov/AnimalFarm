
namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes an object that have a unique identifier.
    /// </summary>
    public interface IHaveId<TId>
    {
        /// <summary>
        /// Gets or sets the objects unique identifier.
        /// </summary>
        TId Id { get; }
    }
}
