using Microsoft.AspNetCore.Http;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils
{
    public enum ServiceType
    {
        Animal,
        Ruleset,
        Authentication
    }

    public class ServiceHttpClient
    {
        const string appTypeName = "AnimalFarm.Server";
        private readonly ServiceType _serviceType;
        private readonly long _partitionKeyHash;

        public ServiceHttpClient(ServiceType serviceType, string partitionKey)
        {
            _serviceType = serviceType;
            _partitionKeyHash = partitionKey.GetHashCode();
        }

        private async Task<string> GetEndpointAsync()
        {
            string serviceTypeName;
            var partitionKey = new ServicePartitionKey(_partitionKeyHash);

            switch (_serviceType)
            {
                // TODO: Extract hardcoded service names.
                case ServiceType.Animal:
                    serviceTypeName = "AnimalFarm.AnimalService";
                    break;
                case ServiceType.Authentication:
                    partitionKey = new ServicePartitionKey();
                    serviceTypeName = "AnimalFarm.AuthenticationService";
                    break;
                case ServiceType.Ruleset:
                    serviceTypeName = "AnimalFarm.RulesetService";
                    break;
                default:
                    throw new NotSupportedException();
            }

            var fabricUri = $"fabric:/{appTypeName}/{serviceTypeName}";
            var resolver = ServicePartitionResolver.GetDefault();
            var p = await resolver.ResolveAsync(new Uri(fabricUri), partitionKey, new System.Threading.CancellationToken());

            JObject addresses = JObject.Parse(p.GetEndpoint().Address);
            return (string)addresses["Endpoints"].First();
        }

        public async Task<TResult> GetAsync<TResult>(string path)
        {
            var uri = await GetEndpointAsync();
            var client = new HttpClient();
            var response = await client.GetAsync($"{uri}{path}");

            // TODO: Handle a failed response.

            var serializer = new JsonSerializer();
            TResult result = JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<HttpResponseMessage> ForwardAsync(string path, HttpRequestMessage request)
        {
            var uri = await GetEndpointAsync();
            var client = new HttpClient();
            var fwRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{uri}/{path}"),
                Method = request.Method,
                Content = request.Content
            };

            return await client.SendAsync(fwRequest);
        }

        public async Task<HttpResponseMessage> ForwardAsync(HttpRequest request, string path)
        {
            var uri = await GetEndpointAsync();
            var client = new HttpClient();
            var fwRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{uri}/{path}"),
                Method = new HttpMethod(request.Method),
                Content = new StreamContent(request.Body)
            };

            if (request.ContentType != null)
                fwRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);

            return await client.SendAsync(fwRequest);
        }
    }
}
