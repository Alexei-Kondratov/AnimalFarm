using AnimalFarm.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
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
                Console.WriteLine($"Deleting table {table.Name} ...");
                await table.DeleteIfExistsAsync();
                Console.WriteLine($"Deleted table {table.Name}");
            }

            table = tableClient.GetTableReference(tableName);

            var created = false;
            Console.WriteLine($"Creating table {table.Name} ...");
            while (!created)
            {
                try
                {
                    await table.CreateIfNotExistsAsync();
                    Console.WriteLine($"Created table {table.Name}");
                    created = true;
                }
                catch (StorageException)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine($"Waiting ...");
                }
            }
        }

        private static async Task Seed(CloudTableClient client, string tableName, IEnumerable<ITableEntity> entities)
        {
            CloudTable table = client.GetTableReference(tableName);

            Console.WriteLine($"Seeding {tableName}");
            foreach (ITableEntity entity in entities)
                await table.ExecuteAsync(TableOperation.Insert(entity));

            Console.WriteLine($"Seeded {entities.Count()} entities in {tableName}");
        }

        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=animalfarm;AccountKey=7Lrjq5wId8TCpSx5o7vFI4nxVugkhjZcOh25RCSp318HIeXDE4o8SkaoVgeb5vKnNtrGXkJapS+Mmuf0Tnp7GA==;EndpointSuffix=core.windows.net";
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            ClearTable(tableClient, "Rulesets").GetAwaiter().GetResult();
            ClearTable(tableClient, "Animals").GetAwaiter().GetResult();
            ClearTable(tableClient, "UserAuthenticationInfos").GetAwaiter().GetResult();

            var seedData = new SeedData();
            Seed(tableClient, "Rulesets", seedData.GetRulesets()).GetAwaiter().GetResult();
            Seed(tableClient, "UserAuthenticationInfos", seedData.GetUserAuthenticationInfos()).GetAwaiter().GetResult();
        }
    }
}
