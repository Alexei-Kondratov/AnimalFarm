//using AnimalFarm.Data.Repositories;
//using AnimalFarm.Model;
//using Xunit;

//namespace AnimalFarm.Service.Utils.Tests
//{
//    /// <summary>
//    /// Unit tests for the RepositoryBuilder class.
//    /// </summary>
//    public class RepositoryBuilderTests
//    {
//        [Fact]
//        public void BuildRepository_creates_an_azure_table_repository()
//        {
//            // Arrange
//            var target = new RepositoryBuilder(@"RepositoriesConfig.json", null);

//            // Act
//            var result = target.BuildRepository<Animal>();

//            // Assert
//            Assert.True(result is AzureTableRepository<Animal>);
//        }

//        [Fact]
//        public void BuildRepository_creates_a_cached_repository()
//        {
//            // Arrange
//            var target = new RepositoryBuilder(@"RepositoriesConfig.json", null);

//            // Act
//            var result = target.BuildRepository<Ruleset>();

//            // Assert
//            Assert.True(result is CachedRepository<Ruleset>);
//        }
//    }
//}
