using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using System.Linq;

namespace AnimalFarm.Logic.AnimalBox
{
    public class AnimalRulesetChangeEventHandler : AnimalEventHandler<AnimalRulesetChangeEvent>
    {
        public override bool Validate(AnimalRulesetChangeEvent e, IAnimalEventContext context)
        {
            return true;
        }

        public override void Apply(AnimalRulesetChangeEvent e, IAnimalEventContext context)
        {
            Animal animal = context.Animal;
            context.ActiveRuleset = context.GetRulesetAsync(e.NewVersionId).GetAwaiter().GetResult();

            AnimalType animalType = context.ActiveRuleset.AnimalTypes[animal.TypeId];
            var newAttributes = animalType.Attributes.Keys.Except(animal.Attributes.Keys);

            foreach (string newAttributeId in newAttributes)
            {
                animal.Attributes.Add(newAttributeId, animalType.Attributes[newAttributeId].InitialValue);
            }
        }
    }

}