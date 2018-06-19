using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;

namespace AnimalFarm.WebUI.Controllers
{
    public class ConfigurationRecord
    {
        public string id { get; set; }
        public string SerializedConfiguration { get; set; }
    }

    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        private const string uri = "https://localhost:8081";
        private const string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        [HttpGet]
        public IEnumerable<ConfigurationRecord> Get()
        {
            var client = new DocumentClient(new Uri(uri), key, new ConnectionPolicy { EnableEndpointDiscovery = false });
            Uri configuraitonCollectionUri = UriFactory.CreateDocumentCollectionUri("AnimalFarm", "Configuration");
            var result = client.CreateDocumentQuery<ConfigurationRecord>(configuraitonCollectionUri).ToList();
            return result;
        }
    }
}
