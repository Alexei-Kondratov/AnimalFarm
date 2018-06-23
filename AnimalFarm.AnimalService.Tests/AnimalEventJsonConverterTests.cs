using AnimalFarm.Model.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace AnimalFarm.AnimalService.Tests
{
    public class AnimalEventJsonConverterTests
    {
        public static IEnumerable<object[]> GetDataForAnimalEventJsonConverter_deserializes_an_animal_event
            => new object[][] {
                new object []
                {
                    JsonConvert.SerializeObject(new CreateAnimalEvent { EventId = "SomeEvent" }).Replace("EventType", "eventType"),
                    typeof(CreateAnimalEvent),
                    "SomeEvent"
                },
                new object []
                {
                    JsonConvert.SerializeObject(new AnimalActionEvent { EventId = "AnotherEvent" }).Replace("Interact", "interact"),
                    typeof(AnimalActionEvent),
                    "AnotherEvent"
                }
            };

        [Theory]
        [MemberData(nameof(GetDataForAnimalEventJsonConverter_deserializes_an_animal_event))]
        public void AnimalEventJsonConverter_deserializes_an_animal_event(string json, Type expectedType, string expectedId)
        {
            // Arrange
            var target = new AnimalEventJsonConverter();
            var jsonReader = new JsonTextReader(new StringReader(json));

            // Act
            var result = target.ReadJson(jsonReader, typeof(AnimalEvent), null, new JsonSerializer()) as AnimalEvent;

            // Assert
            Assert.IsType(expectedType, result);
            Assert.Equal(expectedId, result.EventId);
        }
    }
}
