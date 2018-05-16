using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using AnimalFarm.Model.Tests.Builders;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnimalFarm.Logic.AnimalBox.Tests
{
    public class CreateAnimalEventHandlerTests
    {
        [Fact]
        public void Appply_instantiates_a_new_animal()
        {
            // ArrangeNa
            const string ownerUserId = "UserId";
            const string animalId = "AnimalId";
            const string animalName = "Snowball";
            const string animalTypeId = "PigTypeId";
            const string attributeId = "AttributeId";
            const decimal attributeInitialValue = 30.5M;
            DateTime time = DateTime.UtcNow;

            var ruleset = Build.Ruleset()
                .WithAnimalType(animalTypeId)
                    .HavingAttribute(attributeId, initialValue: attributeInitialValue)
                .And.Finish;

            var target = new CreateAnimalEventHandler();

            CreateAnimalEvent e = new CreateAnimalEvent
            {
                AnimalId = animalId,
                OwnerUserId = ownerUserId,
                AnimalTypeId = animalTypeId,
                Name = animalName,
                Time = time
            };

            var context = new MockAnimalEventContext(null, ruleset);

            // Act
            target.Apply(e, context);

            // Assert
            Animal animal = context.Animal;
            Assert.NotNull(animal);
            Assert.Equal(animalId, animal.Id);
            Assert.Equal(ownerUserId, animal.UserId);
            Assert.Equal(animalTypeId, animal.TypeId);
            Assert.Equal(time, animal.LastCalculated);
            Assert.Equal(attributeInitialValue, animal.Attributes[attributeId]); 
        }
    }
}
