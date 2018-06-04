using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils
{
    [Obsolete]
    public class ServiceHttpClient
    {
        const string _appTypeName = "AnimalFarm.Server";
        private readonly ServiceType _serviceType;
        private readonly ServicePartitionKey _partitionKey;

        public ServiceHttpClient(ServiceType serviceType, string partitionKey)
        {
            _serviceType = serviceType;
            _partitionKey = partitionKey != null ?
                new ServicePartitionKey(partitionKey.GetHashCode())
                : new ServicePartitionKey();

            if (_serviceType == ServiceType.Admin || _serviceType == ServiceType.Authentication)
                _partitionKey = new ServicePartitionKey();
        }

        private static string GetServiceName(ServiceType serviceType)
        {
            switch (serviceType)
            {
                // TODO: Extract hardcoded service names.
                case ServiceType.Admin:
                    return "AnimalFarm.AdminService";
                case ServiceType.Animal:
                    return "AnimalFarm.AnimalService";
                case ServiceType.Authentication:
                    return "AnimalFarm.AuthenticationService";
                case ServiceType.Ruleset:
                    return "AnimalFarm.RulesetService";
                default:
                    throw new NotSupportedException();
            }
        }

        private async Task<string> GetEndpointAsync()
        {
            string serviceTypeName = GetServiceName(_serviceType);

            var fabricUri = $"fabric:/{_appTypeName}/{serviceTypeName}";
            var resolver = ServicePartitionResolver.GetDefault();
            var p = await resolver.ResolveAsync(new Uri(fabricUri), _partitionKey, new System.Threading.CancellationToken());

            JObject addresses = JObject.Parse(p.GetEndpoint().Address);
            return (string)addresses["Endpoints"].First();
        }

        private async Task<Uri> GetUriAsync(ServiceType serviceType, string path)
        {
            var serviceUri = await GetEndpointAsync();
            return new Uri($"{serviceUri}/{path}");
        }

        public async Task<TResult> GetAsync<TResult>(string path)
        {
            var uri = await GetEndpointAsync();
            var client = new HttpClient();
            var response = await client.GetAsync($"{uri}/{path}");

            // TODO: Handle a failed response.

            var serializer = new JsonSerializer();
            TResult result = JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
            return result;
        }
    }
}
