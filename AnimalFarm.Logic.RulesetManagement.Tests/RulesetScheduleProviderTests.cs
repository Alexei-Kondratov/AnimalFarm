using AnimalFarm.Data;
using AnimalFarm.Model;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AnimalFarm.Logic.RulesetManagement.Tests
{
    public class RulesetScheduleProviderTests
    {
        [Fact]
        public async Task GetActiveRulesetRecordsAsync_returns_correct()
        {
            // Arrange
            var rulesetSchedule = new VersionSchedule
            {
                BranchId = "Default",
                Records = new[]
                {
                    new VersionScheduleRecord { VersionId = "1", RulesetId = "First", Start = new DateTime(2018, 1, 1) },
                    new VersionScheduleRecord { VersionId = "2", RulesetId = "Second", Start = new DateTime(2018, 2, 1) },
                    new VersionScheduleRecord { VersionId = "3", RulesetId = "Third", Start = new DateTime(2018, 3, 1) },
                    new VersionScheduleRecord { VersionId = "3", RulesetId = "Fourth", Start = new DateTime(2018, 4, 1) }
                }
            };

            var transactionMock = new Mock<ITransaction>();
            var repositoryMock = new Mock<IRepository<VersionSchedule>>();
            repositoryMock.Setup(_ => _.ByIdAsync(transactionMock.Object, rulesetSchedule.BranchId, rulesetSchedule.BranchId))
                .ReturnsAsync(rulesetSchedule);
            var target = new RulesetScheduleProvider(rulesetSchedule.BranchId, repositoryMock.Object);

            // Act
            var result = await target.GetActiveRulesetRecordsAsync(transactionMock.Object,
                new DateTime(2018, 1, 10), new DateTime(2018, 3, 10));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
