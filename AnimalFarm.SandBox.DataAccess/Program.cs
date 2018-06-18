using AnimalFarm.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Tools
{
    public class Program
    {
        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=775y3kysur4as3;AccountKey=7+kOC+tXA4KgsHLctcbGqThBWbGEyFei46oTNeHhWWtpwAJijuVAXaGIzWf40wX/oW3/6l07Vt438q0q5Netqw==;EndpointSuffix=core.windows.net";
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("WADServiceFabricSystemEventTable");

            var dateTimeOffset = DateTimeOffset.Parse("2018-06-01T00:00:00.702Z");
            var query = new TableQuery().Where(
                TableQuery.GenerateFilterConditionForDate("PreciseTimeStamp", QueryComparisons.GreaterThanOrEqual, dateTimeOffset)
            );

            //var operation = TableOperation.Retrieve("0636632154000000000", "b5d34875-f75c-447b-87a6-311283a907cc___IaaS____gwmje2tb6_0___0000000004295651739");
            var result = table.ExecuteQuerySegmentedAsync(query, null).GetAwaiter().GetResult();
        }
    }
}
