using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Fabric;
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
            return LocateServiceAsync(serviceType, null, cancellationToken);
        }


        public async Task<Uri> LocateServiceAsync(ServiceType serviceType, string partitionKey, CancellationToken cancellationToken)
        {
            string serviceTypeName = GetServiceName(serviceType);

            if (serviceType == ServiceType.Ruleset)
                partitionKey = "";

            var partitionKeyObj = partitionKey != null ?
                new ServicePartitionKey(partitionKey.GetHashCode())
                : new ServicePartitionKey();

            var fabricUri = $"fabric:/{_appTypeName}/{serviceTypeName}";
            var resolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition p = await resolver.ResolveAsync(new Uri(fabricUri), partitionKeyObj, cancellationToken);

            var addresses = JObject.Parse(p.GetEndpoint().Address);
            var result = (string)addresses["Endpoints"].First();
            return new Uri(result);
        }
    }
}
