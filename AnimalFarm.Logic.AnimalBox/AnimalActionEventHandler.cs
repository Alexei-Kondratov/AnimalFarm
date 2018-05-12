using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using System;

namespace AnimalFarm.Logic.AnimalBox
{

    public class AnimalActionEventHandler : AnimalEventHandler<AnimalActionEvent>
    {
        public override bool Validate(AnimalActionEvent e, IAnimalEventContext context)
        {
            return true;
        }

        public override void Apply(AnimalActionEvent e, IAnimalEventContext context)
        {
            Animal animal = context.Animal;
            AnimalAction action = context.ActiveRuleset.AnimalActions[e.AnimalActionId];
            AnimalType animalType = context.ActiveRuleset.AnimalTypes[animal.TypeId];

            foreach (string attributeId in action.AttributeEffects.Keys)
            {
                AnimalTypeAttribute animalTypeAttribute = animalType.Attributes[attributeId];
                decimal minValue = animalTypeAttribute.MinValue;
                decimal maxValue = animalTypeAttribute.MaxValue;
                decimal newValue = animal.Attributes[attributeId] + action.AttributeEffects[attributeId];
                newValue = Math.Max(minValue, Math.Min(maxValue, newValue));
                animal.Attributes[attributeId] = newValue;
            }
        }
    }

}