using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public class ServiceLocator
    {
        const string _appTypeName = "AnimalFarm.Server";

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

        public Task<Uri> LocateServiceAsync(ServiceType serviceType, CancellationToken cancellationToken)
        {
            return LocateServiceAsync(serviceType, (string)null, cancellationToken);
        }

        public async Task<Uri> LocateServiceAsync(ServiceType serviceType, string partitionKey, CancellationToken cancellationToken)
        {
            if (serviceType == ServiceType.Ruleset)
                partitionKey = "";

            var partitionKeyObj = partitionKey != null ?
                new ServicePartitionKey(partitionKey.GetHashCode())
                : new ServicePartitionKey();

            return await LocateServiceAsync(serviceType, partitionKeyObj, cancellationToken);
        }

        private async Task<Uri> LocateServiceAsync(ServiceType serviceType, ServicePartitionKey partitionKey, CancellationToken cancellationToken)
        {
            string serviceTypeName = GetServiceName(serviceType);

            var fabricUri = $"fabric:/{_appTypeName}/{serviceTypeName}";
            var resolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition p = await resolver.ResolveAsync(new Uri(fabricUri), partitionKey, cancellationToken);

            var addresses = JObject.Parse(p.GetEndpoint().Address);
            var result = (string)addresses["Endpoints"].First();
            if (!result.EndsWith('/'))
                result += '/';
            return new Uri(result);
        }

        public async Task<IEnumerable<Uri>> LocateServicesAsync(IEnumerable<ServiceType> serviceTypes, CancellationToken cancellationToken)
        {
            var resolver = ServicePartitionResolver.GetDefault();
            var fabricClient = new FabricClient();
            var result = new ConcurrentBag<Uri>();

            async Task getUri(ServiceType serviceType, Partition partition)
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

                result.Add(await LocateServiceAsync(serviceType, key, cancellationToken));
            }

            async Task<IEnumerable<Task>> getUriTasks(ServiceType serviceType)
            {
                string serviceTypeName = GetServiceName(serviceType);

                ServicePartitionList partitions = await fabricClient.QueryManager.GetPartitionListAsync(new Uri($"fabric:/{_appTypeName}/{serviceTypeName}"));
                return partitions.Select(p => getUri(serviceType, p));
            }

            IEnumerable<Task<IEnumerable<Task>>> getUrisTasks = serviceTypes.Select(getUriTasks);
            IEnumerable<Task> getUris = (await Task.WhenAll(getUrisTasks)).SelectMany(t => t);
            await Task.WhenAll(getUris);

            return result;
        }
    }
}
