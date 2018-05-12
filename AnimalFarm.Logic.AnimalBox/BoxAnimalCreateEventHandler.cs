using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using System;
using System.Linq;

namespace AnimalFarm.Logic.AnimalBox
{

    public class BoxAnimalCreateEventHandler : AnimalEventHandler<AnimalCreateEvent>
    {
        public override bool Validate(AnimalCreateEvent e, IAnimalEventContext context)
        {
            throw new NotImplementedException();
        }

        public override void Apply(AnimalCreateEvent e, IAnimalEventContext context)
        {
            AnimalType animalType = context.ActiveRuleset.AnimalTypes[e.AnimalId];

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