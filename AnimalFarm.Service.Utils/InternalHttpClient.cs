using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils
{
    public enum ServiceType
    {
        Animal,
        Ruleset
    }

    public class ServiceHttpClient
    {
        const string appTypeName = "AnimalFarm.Server";
        private readonly ServiceType _serviceType;
        private readonly string _partitionId;

        public ServiceHttpClient(ServiceType serviceType, string partitionId)
        {
            _serviceType = serviceType;
            _partitionId = partitionId;
        }

        private async Task<string> GetEndpointAsync()
        {
            string serviceTypeName;

            switch (_serviceType)
            {
                // TODO: Extract hardcoded service names.
                case ServiceType.Animal:
                    serviceTypeName = "AnimalFarm.AnimalService";
                    break;
                case ServiceType.Ruleset:
                    serviceTypeName = "AnimalFarm.RulesetService";
                    break;
                default:
                    throw new NotSupportedException();
            }

            var fabricUri = $"fabric:/{appTypeName}/{serviceTypeName}";
            var resolver = ServicePartitionResolver.GetDefault();
            var p = await resolver.ResolveAsync(new Uri(fabricUri), new ServicePartitionKey(_partitionId), new System.Threading.CancellationToken());

            JObject addresses = JObject.Parse(p.GetEndpoint().Address);
            return (string)addresses["Endpoints"].First();
        }

        public async Task<TResult> GetAsync<TResult>(string path)
        {
            var uri = await GetEndpointAsync();
            var client = new HttpClient();
            var response = await client.GetAsync($"{uri}{path}");

            // TODO: Handled a failed response.

            var serializer = new JsonSerializer();
            TResult result = JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
            return result;
        }

    }
}
