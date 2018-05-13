using AnimalFarm.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace AnimalFarm.Tools
{
    public class Program
    {
        private static async Task ClearTable(CloudTable table)
        {
            await table.DeleteIfExistsAsync();
            await table.CreateIfNotExistsAsync();
        }

        private static async Task Seed()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Rulesets");

            var batchSeed = new TableBatchOperation();
            var rulesets = new RulesetSeed().GetRulesets();
            foreach (Ruleset ruleset in rulesets)
                batchSeed.Insert(ruleset);

            await table.ExecuteBatchAsync(batchSeed);
        }

        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=animalfarm;AccountKey=7Lrjq5wId8TCpSx5o7vFI4nxVugkhjZcOh25RCSp318HIeXDE4o8SkaoVgeb5vKnNtrGXkJapS+Mmuf0Tnp7GA==;EndpointSuffix=core.windows.net";
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("Rulesets");
            ClearTable(table).GetAwaiter().GetResult();

            var operation = TableOperation.Retrieve<Ruleset>("Rules", "BaseRuleset");
            var result = table.ExecuteAsync(operation).GetAwaiter().GetResult();
        }
    }
}
