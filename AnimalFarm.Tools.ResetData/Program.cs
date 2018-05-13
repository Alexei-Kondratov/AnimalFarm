using AnimalFarm.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Tools.ResetData
{
    public class Program
    {
        private static async Task ClearTable(CloudTableClient tableClient, string tableName)
        {
            CloudTable table = tableClient.GetTableReference(tableName);

            if (await table.ExistsAsync())
            {
                Console.WriteLine($"Deleting table {table.Name}");
                await table.DeleteIfExistsAsync();
                Console.WriteLine($"Deleted table {table.Name}");
            }

            while (await table.ExistsAsync())
                Thread.Sleep(100);

            table = tableClient.GetTableReference(tableName);

            Console.WriteLine($"Creating table {table.Name}");
            await table.CreateIfNotExistsAsync();
            Console.WriteLine($"Created table {table.Name}");
        }

        private static async Task Seed(CloudTableClient tableClient)
        {
            CloudTable table = tableClient.GetTableReference("Rulesets");

            var rulesets = new RulesetSeed().GetRulesets();
            Console.WriteLine($"Seeding rulesets");
            foreach (Ruleset ruleset in rulesets)
                await table.ExecuteAsync(TableOperation.Insert(ruleset));

            Console.WriteLine($"Seeded {rulesets.Count()} rulesets");
        }

        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=animalfarm;AccountKey=7Lrjq5wId8TCpSx5o7vFI4nxVugkhjZcOh25RCSp318HIeXDE4o8SkaoVgeb5vKnNtrGXkJapS+Mmuf0Tnp7GA==;EndpointSuffix=core.windows.net";
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            ClearTable(tableClient, "Rulesets").GetAwaiter().GetResult();
            ClearTable(tableClient, "Animals").GetAwaiter().GetResult();
            Seed(tableClient).GetAwaiter().GetResult();
        }
    }
}
