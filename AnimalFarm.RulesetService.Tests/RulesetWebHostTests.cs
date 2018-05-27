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
using System;
using System.Threading.Tasks;
using AnimalFarm.Logic.RulesetManagement;

namespace AnimalFarm.RulesetService.Tests
{
    public class RulesetWebHostTests
    {
        [Theory]
        [InlineData(-10, -5, "NewRuleset")]
        [InlineData(-10, 10, "OldRuleset")]
        public async Task Get_returns_the_current_ruleset(int oldRulesetTimeDeltaMin, int newRulesetTimeDeltaMin, string expectedRulesetId)
        {
            // Arrange
            var transactionMock = new Mock<ITransaction>();
            var transactionManagerMock = new Mock<ITransactionManager>();
            var rulesetRepositoryMock = new Mock<IRepository<Ruleset>>();
            var scheduleRepositoryMock = new Mock<IRepository<VersionSchedule>>();

            Ruleset oldRuleset = Build.Ruleset("OldRuleset");
            Ruleset newRuleset = Build.Ruleset("NewRuleset")
                .WithAnimalAction("AnimalAction")
                .And.WithAnimalType("AnimalType")
                .And.Finish;

            var rulesetSchedule = new VersionSchedule
            {
                BranchId = "Default",
                Records = new[]
                {
                    new VersionScheduleRecord { VersionId = "1", RulesetId = "OldRuleset", Start = DateTime.UtcNow + TimeSpan.FromMinutes(oldRulesetTimeDeltaMin) },
                    new VersionScheduleRecord { VersionId = "2", RulesetId = "NewRuleset", Start = DateTime.UtcNow + TimeSpan.FromMinutes(newRulesetTimeDeltaMin) },
                }
            };

            transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(transactionMock.Object);
            rulesetRepositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, oldRuleset.Id, oldRuleset.Id)).ReturnsAsync(oldRuleset);
            rulesetRepositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, newRuleset.Id, newRuleset.Id)).ReturnsAsync(newRuleset);
            scheduleRepositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, rulesetSchedule.BranchId, rulesetSchedule.BranchId)).ReturnsAsync(rulesetSchedule);

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services => services
                    .AddSingleton(transactionManagerMock.Object)
                    .AddSingleton(rulesetRepositoryMock.Object)
                    .AddSingleton(scheduleRepositoryMock.Object)
                    .AddSingleton(new RulesetScheduleProvider("Default", scheduleRepositoryMock.Object)))
                .UseStartup<Startup>());
            var client = server.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("");

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedRuleset = JsonConvert.DeserializeObject<Ruleset>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(returnedRuleset);
            Assert.Equal(expectedRulesetId, returnedRuleset.Id);
        }      
    }
}
