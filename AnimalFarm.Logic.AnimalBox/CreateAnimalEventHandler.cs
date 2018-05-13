using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using System;
using System.Linq;

namespace AnimalFarm.Logic.AnimalBox
{
    /// <summary>
    /// Processes the CreateAnimalEvent avent.
    /// </summary>
    public class CreateAnimalEventHandler : AnimalEventHandler<CreateAnimalEvent>
    {
        public override bool Validate(CreateAnimalEvent e, IAnimalEventContext context)
        {
            return true;
        }

        public override void Apply(CreateAnimalEvent e, IAnimalEventContext context)
        {
            AnimalType animalType = context.ActiveRuleset.AnimalTypes[e.AnimalTypeId];

            var animal = new Animal
            {
                Id = e.AnimalId,
                Name = e.Name,
                UserId = e.OwnerUserId,
                TypeId = e.AnimalTypeId,
                Attributes = animalType.Attributes.ToDictionary(a => a.Key, a => a.Value.InitialValue),
                LastCalculated = e.Time,
                Created = e.Time
            };

            context.Animal = animal;
        }
    }

}