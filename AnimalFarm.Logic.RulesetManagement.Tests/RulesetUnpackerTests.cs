using AnimalFarm.Data;
using AnimalFarm.Model;
using AnimalFarm.Model.Tests.Builders;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AnimalFarm.Logic.RulesetManagement.Tests
{
    public class RulesetUnpackerTests
    {
        [Fact]
        public async Task Unpacks_resolves_inheritance()
        {
            // Arrange
            const string baseId = "BaseRulesetId";
            var baseRuleset = Build.Ruleset(baseId)
                .WithAnimalAction("AnimalActionId")
                .And.WithAnimalType("AnimalTypeId")
                .And.Finish;

            const string childId = "ChildRulesetId";
            var ruleset = Build.Ruleset(childId)
                .Inheriting(baseId)
                .WithAnimalAction("AnimalActionId")
                .And.WithAnimalType("AnotherAnimalTypeId")
                .And.Finish;

            var transactionMock = new Mock<ITransaction>();
            var repositoryMock = new Mock<IRepository<Ruleset>>();
            repositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, baseId, baseId)).ReturnsAsync(baseRuleset);
            repositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, childId, childId)).ReturnsAsync(ruleset);
            var target = new RulesetUnpacker(repositoryMock.Object);

            // Act
            var result = await target.UnpackAsync(transactionMock.Object, ruleset);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(childId, result.Id);
            Assert.Single(result.AnimalActions);
            Assert.Equal(2, result.AnimalTypes.Count);
        }
    }
}
