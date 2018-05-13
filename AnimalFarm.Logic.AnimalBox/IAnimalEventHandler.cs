using AnimalFarm.Model.Events;

namespace AnimalFarm.Logic.AnimalBox
{
    /// <summary>
    /// Contract for classes responsible for handling animal events.
    /// </summary>
    public interface IAnimalEventHandler
    {
        bool Validate(AnimalEvent e, IAnimalEventContext context);
        void Apply(AnimalEvent e, IAnimalEventContext context);
    }
}