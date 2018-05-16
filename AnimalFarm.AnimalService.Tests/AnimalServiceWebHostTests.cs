using AnimalFarm.Data;
using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using AnimalFarm.Model.Tests.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnimalFarm.AnimalService.Tests
{
    public class AnimalServiceWebHostTests
    {
        private Mock<ITransaction> _transactionMock = new Mock<ITransaction>();
        private Mock<ITransactionManager> _transactionManagerMock = new Mock<ITransactionManager>();
        private Mock<IRepository<Animal>> _animalRepositoryMock = new Mock<IRepository<Animal>>();
        private Mock<IRepository<Ruleset>> _rulesetRepositoryMock = new Mock<IRepository<Ruleset>>();

        private HttpClient CreateTestClient()
        {
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services => services
                    .AddSingleton(_transactionManagerMock.Object)
                    .AddSingleton(_animalRepositoryMock.Object)
                    .AddSingleton(_rulesetRepositoryMock.Object))
                .UseStartup<Startup>());
            return server.CreateClient();
        }

        [Fact]
        public void Get_animal_returns_an_animal_with_the_given_id()
        {
            // Arrange
            var animal = new Animal
            {
                Id = "AnimalId",
                Name = "Snowball",
                UserId = "UserId",
                Attributes = new Dictionary<string, decimal>
                {
                    { "AttributeId", 10 }
                }
            };

            _transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(_transactionMock.Object);
            _animalRepositoryMock.Setup(_ => _.ByIdAsync(_transactionMock.Object, animal.UserId, animal.Id)).ReturnsAsync(animal);

            var client = CreateTestClient();

            // Act
            HttpResponseMessage response = client.GetAsync($"{animal.UserId}/{animal.Id}").GetAwaiter().GetResult();

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContentString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var returnedAnimal = JsonConvert.DeserializeObject<Animal>(responseContentString);

            Assert.NotNull(returnedAnimal);
            Assert.Equal(animal.Id, returnedAnimal.Id);
            Assert.Equal(animal.Name, returnedAnimal.Name);
            Assert.Equal(animal.Attributes.Count, returnedAnimal.Attributes.Count);
            Assert.Equal(animal.Attributes.First().Key, returnedAnimal.Attributes.First().Key);
            Assert.Equal(animal.Attributes.First().Value, returnedAnimal.Attributes.First().Value);
        }

        [Fact]
        public void Event_create_animal_creates_a_new_animal()
        {
            // Arrange
            var ruleset = Build.Ruleset("BaseRuleset")
                .WithAnimalType("HorseId")
                    .HavingAttribute("HappinessId", initialValue: 33)
                .And.Finish;

            var e = new CreateAnimalEvent
            {
                EventId = "EventId",
                ActingUserId = "UserId",
                OwnerUserId = "UserId",
                AnimalId = "AnimalId",
                Name = "Boxer",
                AnimalTypeId = "HorseId",
                Time = DateTime.UtcNow
            };

            Animal newAnimal = null;
            Action<ITransaction, Animal> setNewAnimal = (tx, a) => newAnimal = a;
            _transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(_transactionMock.Object);
            _rulesetRepositoryMock.Setup(_ => _.ByIdAsync(_transactionMock.Object, ruleset.Id, ruleset.Id)).ReturnsAsync(ruleset);
            _animalRepositoryMock.Setup(_ => _.ByIdAsync(_transactionMock.Object, e.OwnerUserId, e.AnimalId)).ReturnsAsync((Animal)null);
            _animalRepositoryMock.Setup(_ => _.UpsertAsync(_transactionMock.Object, It.IsAny<Animal>())).Callback(setNewAnimal)
                .Returns(Task.CompletedTask);

            var client = CreateTestClient();
            var stringContent = new StringContent(JsonConvert.SerializeObject(e, typeof(AnimalEvent), Formatting.None,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }), Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = client.PutAsync($"event", stringContent).GetAwaiter().GetResult();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.NotNull(newAnimal);
            Assert.Equal(e.AnimalId, newAnimal.Id);
            Assert.Equal(e.Name, newAnimal.Name);
            var animalTypeAttribute = ruleset.AnimalTypes[e.AnimalTypeId].Attributes.First();
            Assert.Equal(animalTypeAttribute.Key, newAnimal.Attributes.First().Key);
            Assert.Equal(animalTypeAttribute.Value.InitialValue, newAnimal.Attributes.First().Value);
        }

        [Fact]
        public void Event_animal_action_updates_the_animal()
        {
            // Arrange
            const int startingAttributeValue = 27;
            var animal = new Animal
            {
                Id = "AnimalId",
                Name = "Snowball",
                UserId = "UserId",
                TypeId = "PigId",
                Attributes = new Dictionary<string, decimal>
                {
                    { "Hunger", startingAttributeValue }
                },
                LastCalculated = new DateTime(2018, 5, 1)
            };

            var ruleset = Build.Ruleset("BaseRuleset")
                .WithAnimalType(animal.TypeId)
                    .HavingAttribute(animal.Attributes.First().Key, ratio: 10)
                .And.WithAnimalAction("FeedId")
                    .HavingAttributeEffect(animal.Attributes.First().Key, -20)
                .And.Finish;

            var e = new AnimalActionEvent
            {
                EventId = "EventId",
                ActingUserId = animal.UserId,
                OwnerUserId = animal.UserId,
                AnimalId = animal.Id,
                AnimalActionId = ruleset.AnimalActions.First().Key,
                Time = animal.LastCalculated.AddMinutes(1)
            };

            Animal updatedAnimal = null;
            Action<ITransaction, Animal> setNewAnimal = (tx, a) => updatedAnimal = a;
            _transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(_transactionMock.Object);
            _rulesetRepositoryMock.Setup(_ => _.ByIdAsync(_transactionMock.Object, ruleset.Id, ruleset.Id)).ReturnsAsync(ruleset);
            _animalRepositoryMock.Setup(_ => _.ByIdAsync(_transactionMock.Object, e.OwnerUserId, e.AnimalId)).ReturnsAsync(animal);
            _animalRepositoryMock.Setup(_ => _.UpsertAsync(_transactionMock.Object, It.IsAny<Animal>())).Callback(setNewAnimal)
                .Returns(Task.CompletedTask);

            var client = CreateTestClient();
            var stringContent = new StringContent(JsonConvert.SerializeObject(e, typeof(AnimalEvent), Formatting.None,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }), Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = client.PutAsync($"event", stringContent).GetAwaiter().GetResult();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.NotNull(updatedAnimal);
            Assert.Equal(e.AnimalId, updatedAnimal.Id);
            var expectedAttributeValue = startingAttributeValue
                + ruleset.AnimalTypes[animal.TypeId].Attributes.First().Value.RatioPerMinute
                + ruleset.AnimalActions[e.AnimalActionId].AttributeEffects.First().Value;
            Assert.Equal(expectedAttributeValue, updatedAnimal.Attributes.First().Value);
            Assert.Equal(e.Time, updatedAnimal.LastCalculated);
        }
    }
}
