using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using AnimalFarm.Model.Tests.Builders;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnimalFarm.Logic.AnimalBox.Tests
{
    public class AnimalRulesetChangeEventHandlerTests
    {
        [Fact]
        public void Apply_adds_new_attribute()
        {
            // Arrange
            const string animalId = "AnimalId";
            const string animalTypeId = "AnimalTypeId";
            const string attributeId = "AttributeId";
            const string newAttributeId = "NewAttributeId";
            const string newRulesetId = "NewRuleset";
            const int newAttributeInitialValue = 23;

            var animal = new Animal
            {
                Id = animalId,
                TypeId = animalTypeId,
                Attributes = new Dictionary<string, decimal>
                {
                    { attributeId, 0 }
                }
            };

            var oldRuleset = Build.Ruleset("OldRuleset")
                .WithAnimalType(animalTypeId)
                    .HavingAttribute(attributeId)
                .And.Finish;
            var newRuleset = Build.Ruleset(newRulesetId)
                .WithAnimalType(animalTypeId)
                    .HavingAttribute(attributeId)
                    .HavingAttribute(newAttributeId, initialValue: newAttributeInitialValue)
                .And.Finish;

            var target = new AnimalRulesetChangeEventHandler();

            var e = new AnimalRulesetChangeEvent { AnimalId = animalId, NewVersionId = newRulesetId };
            var mockAnimalContext = new MockAnimalEventContext(animal, oldRuleset, newRuleset);

            // Act
            target.Apply(e, mockAnimalContext);

            // Assert
            Assert.Contains(newAttributeId, animal.Attributes.Keys);
            Assert.Equal(newAttributeInitialValue, animal.Attributes[newAttributeId]);
        }
    }
}
