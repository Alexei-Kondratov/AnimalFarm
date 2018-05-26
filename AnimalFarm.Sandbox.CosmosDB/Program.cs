using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Sandbox.CosmosDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
        }

        public static void Initialize()
        {
            var client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

            var response1 = client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri("AnimalFarm")).GetAwaiter().GetResult();
            var response2 = client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("AnimalFarm", "Rulesets")).GetAwaiter().GetResult();

        }

        private static async Task CreateDatabaseIfNotExistsAsync(DocumentClient client, string databaseId)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync(DocumentClient client, string databaseId, string collectionId)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        new DocumentCollection { Id = collectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
