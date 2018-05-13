using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using AnimalFarm.Model.Tests.Builders;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnimalFarm.Logic.AnimalBox.Tests
{
    public class AnimalActionEventHandlerTests
    {
        [Theory]
        [InlineData(50, -10, 40)]
        [InlineData(15, 15, 30)]
        [InlineData(80, 30, 100, 0, 100)]
        [InlineData(10, -30, 0, 0, 100)]
        public void Appply_applies_the_modifier_for_a_single_attribute_action
            (decimal initialValue, decimal effect, decimal expectedValue,
                decimal minValue = 0, decimal maxValue = 100)
        {
            // Arrange
            const string animalId = "AnimalId";
            const string animalTypeId = "AnimalTypeId";
            const string attributeId = "AttributeId";
            const string animalActionId = "AnimalActionId";

            var animal = new Animal
            {
                Id = animalId,
                TypeId = animalTypeId,
                Attributes = new Dictionary<string, decimal>
                {
                    { attributeId, initialValue }
                }
            };

            var ruleset = Build.Ruleset()
                .WithAnimalAction(animalActionId)
                    .HavingAttributeEffect(attributeId, effect)
                .And.WithAnimalType(animalTypeId)
                    .HavingAttribute(attributeId, minValue: minValue, maxValue: maxValue)
                .And.Finish;

            var target = new AnimalActionEventHandler();

            AnimalActionEvent e = new AnimalActionEvent { AnimalId = animalId, AnimalActionId = animalActionId };

            // Act
            target.Apply(e, new MockAnimalEventContext { Animal = animal, ActiveRuleset = ruleset });

            // Assert
            Assert.Equal(expectedValue, animal.Attributes[attributeId]);
        }
    }
}
