using AnimalFarm.Model.Events;

namespace AnimalFarm.Logic.AnimalBox
{

    public interface IAnimalEventHandler
    {
        bool Validate(AnimalEvent e, IAnimalEventContext context);
        void Apply(AnimalEvent e, IAnimalEventContext context);
    }

}