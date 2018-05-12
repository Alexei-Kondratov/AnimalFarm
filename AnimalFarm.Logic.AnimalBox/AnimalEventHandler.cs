using AnimalFarm.Model.Events;

namespace AnimalFarm.Logic.AnimalBox
{
    public abstract class AnimalEventHandler<TEvent> : IAnimalEventHandler
    where TEvent : AnimalEvent
    {
        public abstract bool Validate(TEvent e, IAnimalEventContext context);

        public abstract void Apply(TEvent e, IAnimalEventContext context);

        bool IAnimalEventHandler.Validate(AnimalEvent e, IAnimalEventContext context)
        {
            if (e is TEvent)
            {
                return Validate(e as TEvent, context);
            }

            return true;
        }

        void IAnimalEventHandler.Apply(AnimalEvent e, IAnimalEventContext context)
        {
            if (e is TEvent)
            {
                Apply(e as TEvent, context);
            }
        }
    }

}