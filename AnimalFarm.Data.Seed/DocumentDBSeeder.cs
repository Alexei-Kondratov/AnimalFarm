using AnimalFarm.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalFarm.Data.Seed
{
    public class DocumentDBSeeder : IDataSeeder
    {
        private static async Task ClearTableAsync(DocumentClient client, string databaseId, string tableName, string partitionPath)
        {
            var collection = new DocumentCollection { Id = tableName };
            collection.PartitionKey.Paths.Add(partitionPath);

            try
            {
                await client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, tableName));
            }
            catch
            {
            }

            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId), collection);
        }

        private static async Task SeedAsync(DocumentClient client, string databaseId, string tableName, IEnumerable<IHavePartition<string, string>> entities)
        {
            foreach (var entity in entities)
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, tableName), entity,
                    new RequestOptions { PartitionKey = new PartitionKey(entity.PartitionKey) },
                    disableAutomaticIdGeneration: true);
        }

        public async Task SeedAsync(SeedData seedData)
        {
            var client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", new ConnectionPolicy { EnableEndpointDiscovery = false });
            var database = new Database { Id = "AnimalFarm" };
            await client.CreateDatabaseIfNotExistsAsync(database);

            foreach (SeedCollection collection in seedData.Collections)
            {
                await ClearTableAsync(client, database.Id, collection.Name, $"/{nameof(IHavePartition<string, string>.PartitionKey)}");
                await SeedAsync(client, database.Id, collection.Name, collection.Entities);
            }
        }
    }
}
