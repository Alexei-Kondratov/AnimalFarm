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
        [Fact]
        public void Get_animal_returns_an_animal_with_the_given_id()
        {
            // Arrange
            var transactionMock = new Mock<ITransaction>();
            var transactionManagerMock = new Mock<ITransactionManager>();
            var animalRepositoryMock = new Mock<IRepository<Animal>>();
            var rulesetRepositoryMock = new Mock<IRepository<Ruleset>>();

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

            transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(transactionMock.Object);
            animalRepositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, animal.UserId, animal.Id)).ReturnsAsync(animal);

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services => services
                    .AddSingleton(transactionManagerMock.Object)
                    .AddSingleton(animalRepositoryMock.Object)
                    .AddSingleton(rulesetRepositoryMock.Object))
                .UseStartup<Startup>());
            var client = server.CreateClient();

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
            var transactionMock = new Mock<ITransaction>();
            var transactionManagerMock = new Mock<ITransactionManager>();
            var animalRepositoryMock = new Mock<IRepository<Animal>>();
            var rulesetRepositoryMock = new Mock<IRepository<Ruleset>>();

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
            transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(transactionMock.Object);
            rulesetRepositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, ruleset.Id, ruleset.Id)).ReturnsAsync(ruleset);
            animalRepositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, e.OwnerUserId, e.AnimalId)).ReturnsAsync((Animal)null);
            animalRepositoryMock.Setup(_ => _.UpsertAsync(transactionMock.Object, It.IsAny<Animal>())).Callback(setNewAnimal)
                .Returns(Task.CompletedTask);

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services => services
                    .AddSingleton(transactionManagerMock.Object)
                    .AddSingleton(animalRepositoryMock.Object)
                    .AddSingleton(rulesetRepositoryMock.Object))
                .UseStartup<Startup>());
            var client = server.CreateClient();

            var stringContent = new StringContent(JsonConvert.SerializeObject(e), Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = client.PutAsync($"event/{e.EventId}", stringContent).GetAwaiter().GetResult();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.NotNull(newAnimal);
            Assert.Equal(e.AnimalId, newAnimal.Id);
            Assert.Equal(e.Name, newAnimal.Name);
            var animalTypeAttribute = ruleset.AnimalTypes[e.AnimalTypeId].Attributes.First();
            Assert.Equal(animalTypeAttribute.Key, newAnimal.Attributes.First().Key);
            Assert.Equal(animalTypeAttribute.Value.InitialValue, newAnimal.Attributes.First().Value);
        }
    }
}
