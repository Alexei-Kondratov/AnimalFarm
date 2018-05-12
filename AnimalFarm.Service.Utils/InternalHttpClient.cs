using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils
{
    public enum InternalService
    {
        Animal,
        Ruleset
    }

    public class InternalHttpClient
    {
        const string appTypeName = "AnimalFarm.Server";

        private async Task<string> GetEndpointAsync(InternalService service)
        {
            string serviceTypeName;

            switch (service)
            {
                case InternalService.Animal:
                    serviceTypeName = "AnimalFarm.AnimalService";
                    break;
                case InternalService.Ruleset:
                    serviceTypeName = "AnimalFarm.RulesetService";
                    break;
                default:
                    throw new NotSupportedException();
            }

            var fabricUri = $"fabric:/{appTypeName}/{serviceTypeName}";
            var resolver = ServicePartitionResolver.GetDefault();
            var p = await resolver.ResolveAsync(new Uri(fabricUri), new ServicePartitionKey(), new System.Threading.CancellationToken());

            JObject addresses = JObject.Parse(p.GetEndpoint().Address);
            return (string)addresses["Endpoints"].First();
        }

        public async Task<TResult> GetAsync<TResult>(InternalService service, string path)
        {
            var uri = await GetEndpointAsync(service);
            var client = new HttpClient();
            var response = await client.GetAsync($"{uri}{path}");

            var serializer = new JsonSerializer();
            TResult result = JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
            return result;
        }

    }
}
