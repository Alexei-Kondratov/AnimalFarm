using Microsoft.AspNetCore.Http;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils
{
    public enum ServiceType
    {
        Animal,
        Ruleset,
        Authentication,
        Admin
    }

    public class ServiceHttpClient
    {
        const string _appTypeName = "AnimalFarm.Server";
        private readonly ServiceType _serviceType;
        private readonly long _partitionKeyHash;

        public ServiceHttpClient(ServiceType serviceType, string partitionKey)
        {
            _serviceType = serviceType;
            _partitionKeyHash = partitionKey.GetHashCode();
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
            var partitionKey = new ServicePartitionKey(_partitionKeyHash);

            if (_serviceType == ServiceType.Admin || _serviceType == ServiceType.Authentication)
                partitionKey = new ServicePartitionKey();

            var fabricUri = $"fabric:/{_appTypeName}/{serviceTypeName}";
            var resolver = ServicePartitionResolver.GetDefault();
            var p = await resolver.ResolveAsync(new Uri(fabricUri), partitionKey, new System.Threading.CancellationToken());

            JObject addresses = JObject.Parse(p.GetEndpoint().Address);
            return (string)addresses["Endpoints"].First();
        }

        private async Task<Uri> GetUriAsync(ServiceType serviceType, string path)
        {
            var serviceUri = await GetEndpointAsync();
            return new Uri($"{serviceUri}/{path}");
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object payload = null, Type payloadType = null)
        {
            var uri = await GetUriAsync(_serviceType, path);
            var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = method
            };

            if (payload != null)
            {
                payloadType = payloadType ?? payload.GetType();
                var stringContent = new StringContent(JsonConvert.SerializeObject(payload, payloadType, Formatting.None,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }), Encoding.UTF8, "application/json");

                request.Content = stringContent;
            }

            return await client.SendAsync(request);
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

        public async Task<HttpResponseMessage> ForwardAsync(string path, HttpRequestMessage request)
        {
            var uri = await GetUriAsync(_serviceType, path);
            var client = new HttpClient();

            var fwRequest = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = request.Method,
                Content = request.Content
            };

            return await client.SendAsync(fwRequest);
        }

        public async Task<HttpResponseMessage> ForwardAsync(HttpRequest request, string path)
        {
            var uri = await GetUriAsync(_serviceType, path);
            var client = new HttpClient();

            var fwRequest = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = new HttpMethod(request.Method),
                Content = new StreamContent(request.Body)
            };

            if (request.ContentType != null)
                fwRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);

            return await client.SendAsync(fwRequest);
        }

        public async Task BroadcastAsync(IEnumerable<ServiceType> serviceTypes, HttpMethod method, string path, object payload = null, Type payloadType = null)
        {
            var resolver = ServicePartitionResolver.GetDefault();
            var fabricClient = new FabricClient();

            foreach (ServiceType serviceType in serviceTypes)
            {
                string serviceTypeName = GetServiceName(serviceType);

                var partitions = await fabricClient.QueryManager.GetPartitionListAsync(new Uri($"fabric:/{_appTypeName}/{serviceTypeName}"));
                foreach (Partition partition in partitions)
                {
                    ServicePartitionKey key;
                    switch (partition.PartitionInformation.Kind)
                    {
                        case ServicePartitionKind.Singleton:
                            key = ServicePartitionKey.Singleton;
                            break;
                        case ServicePartitionKind.Int64Range:
                            var longKey = (Int64RangePartitionInformation)partition.PartitionInformation;
                            key = new ServicePartitionKey(longKey.LowKey);
                            break;
                        case ServicePartitionKind.Named:
                            var namedKey = (NamedPartitionInformation)partition.PartitionInformation;
                            key = new ServicePartitionKey(namedKey.Name);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("partition.PartitionInformation.Kind");
                    }

                    var client = new ServiceHttpClient(serviceType, key.ToString());

                    await client.SendAsync(method, path, payload, payloadType);
                }
            }
        }
    }
}
