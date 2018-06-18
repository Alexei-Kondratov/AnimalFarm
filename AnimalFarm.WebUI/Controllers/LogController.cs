using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.WebUI.Controllers
{
    public class LogRecord : TableEntity
    {
        public DateTime PreciseTimeStamp { get; set; }
        public string EventMessage { get; set; }
        public string Message { get; set; }
    }

    [Route("api/[controller]")]
    public class LogController : Controller
    {
        private const string connectionString =
          "DefaultEndpointsProtocol=https;AccountName=775y3kysur4as3;AccountKey=7+kOC+tXA4KgsHLctcbGqThBWbGEyFei46oTNeHhWWtpwAJijuVAXaGIzWf40wX/oW3/6l07Vt438q0q5Netqw==;EndpointSuffix=core.windows.net";

        [HttpGet]
        public async Task<IEnumerable<LogRecord>> Get()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("WADServiceFabricSystemEventTable");

            var query = new TableQuery<LogRecord>()
            .Where(
                TableQuery.GenerateFilterConditionForDate("PreciseTimeStamp", QueryComparisons.GreaterThanOrEqual, new DateTimeOffset(new DateTime(2018, 06, 01)))
            );

            TableQuerySegment<LogRecord> result = await table.ExecuteQuerySegmentedAsync(query, null);
            return result.OrderByDescending(l => l.PreciseTimeStamp).Take(100);
        }
    }
}
