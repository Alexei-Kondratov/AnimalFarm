using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http;
using Xunit;
using AnimalFarm.Data;
using AnimalFarm.Model;
using Newtonsoft.Json;
using AnimalFarm.Model.Tests.Builders;
using System.Linq;

namespace AnimalFarm.RulesetService.Tests
{
    public class RulesetWebHostTests
    {
        [Fact]
        public void Get_returns_the_current_ruleset()
        {
            // Arrange
            var transactionMock = new Mock<ITransaction>();
            var transactionManagerMock = new Mock<ITransactionManager>();
            var rulesetRepositoryMock = new Mock<IRepository<Ruleset>>();

            var ruleset = Build.Ruleset("BaseRuleset")
                .WithAnimalAction("AnimalAction")
                .And.WithAnimalType("AnimalType")
                .And.Finish;

            transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(transactionMock.Object);
            rulesetRepositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, ruleset.Id)).ReturnsAsync(ruleset);

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services => services
                    .AddSingleton(transactionManagerMock.Object)
                    .AddSingleton(rulesetRepositoryMock.Object))
                .UseStartup<Startup>());
            var client = server.CreateClient();

            // Act
            HttpResponseMessage response = client.GetAsync("").GetAwaiter().GetResult();

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedRuleset = JsonConvert.DeserializeObject<Ruleset>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            Assert.NotNull(returnedRuleset);
            Assert.Equal(ruleset.Id, returnedRuleset.Id);
            Assert.Equal(ruleset.AnimalActions.Count(), returnedRuleset.AnimalActions.Count());
            Assert.Equal(ruleset.AnimalActions.First().Key, returnedRuleset.AnimalActions.First().Key);
            Assert.Equal(ruleset.AnimalTypes.Count(), returnedRuleset.AnimalTypes.Count());
            Assert.Equal(ruleset.AnimalTypes.First().Key, returnedRuleset.AnimalTypes.First().Key);
        }
    }
}
