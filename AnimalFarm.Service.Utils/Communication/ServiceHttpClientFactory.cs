using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public class ServiceHttpClientFactory : IServiceHttpClientFactory
    {
        private readonly ServiceLocator _serviceLocator;

        public ServiceHttpClientFactory(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public async Task<IServiceHttpClient> CreateAsync(ServiceType serviceType, string partitionKey, CancellationToken cancellationToken)
        {
            Uri serviceUri = await _serviceLocator.LocateServiceAsync(serviceType, partitionKey, cancellationToken);
            return new ServiceHttpClient(serviceUri);
        }

        public Task<IServiceHttpClient> CreateAsync(ServiceType service, CancellationToken cancellationToken)
        {
            return CreateAsync(service, null, cancellationToken);
        }
    }
}
