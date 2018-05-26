using AnimalFarm.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Tools.ResetData
{
    public static class DocumentDBSeed
    {
        private static async Task ClearTableAsync(DocumentClient client, string databaseId, string tableName)
        {
            var collection = new DocumentCollection { Id = tableName };

            Console.WriteLine($"Creating table {tableName} ...");
            var result = await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId), collection);
            Console.WriteLine($"Created table {tableName}");
        }

        private static async Task SeedAsync(DocumentClient client, string databaseId, string tableName, IEnumerable<IHaveId<string>> entities)
        {
            foreach (var entity in entities)
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, tableName), entity,
                    disableAutomaticIdGeneration: true);

            Console.WriteLine($"Seeded {entities.Count()} entities in {tableName}");
        }

        public async static Task SeedAsync()
        {
            var client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", new ConnectionPolicy { EnableEndpointDiscovery = false });
            var database = new Database { Id = " AnimalFarm" };
            await client.CreateDatabaseIfNotExistsAsync(database);


            await ClearTableAsync(client, database.Id, "Rulesets");
            await ClearTableAsync(client, database.Id, "Animals");
            await ClearTableAsync(client, database.Id, "UserAuthenticationInfos");

            var seedData = new SeedData();
            await SeedAsync(client, database.Id, "Rulesets", seedData.GetRulesets());
            await SeedAsync(client, database.Id, "UserAuthenticationInfos", seedData.GetUserAuthenticationInfos());
        }
    }
}
