using AnimalFarm.Utils.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;

namespace AnimalFarm.WebUI.Controllers
{
    class Entity
    {
        public string id { get; set; }
        public string PartitionKey { get; set; }
    }

    [Route("api/[controller]")]
    public class AnimalController : Controller
    {
        private const string uri = "https://localhost:8081";
        private const string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        [HttpGet]
        public IActionResult Animals()
        {
            string userId = new JwtManager().ValidateToken(Request.Headers["User-Token"]);
            var client = new DocumentClient(new Uri(uri), key);

            var response1 = client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri("AnimalFarm")).GetAwaiter().GetResult();
            var query = client.CreateDocumentQuery<Entity>(UriFactory.CreateDocumentCollectionUri("AnimalFarm", "Animals")).
                Where(e => e.PartitionKey == userId).Select(e => e.id);

            var result = query.ToList();
            return Json(result);
        }
    }
}