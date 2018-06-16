using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public class ServiceHttpClientFactory : IServiceHttpClientFactory
    {
        private readonly IRequestContextAccessor _requestContextAccessor;
        private readonly ServiceLocator _serviceLocator;

        public ServiceHttpClientFactory(ServiceLocator serviceLocator, IRequestContextAccessor requestContextAccessor)
        {
            _serviceLocator = serviceLocator;
            _requestContextAccessor = requestContextAccessor;
        }

        public async Task<IServiceHttpClient> CreateAsync(ServiceType serviceType, string partitionKey, CancellationToken cancellationToken)
        {
            Uri serviceUri = await _serviceLocator.LocateServiceAsync(serviceType, partitionKey, cancellationToken);
            return new ServiceHttpClient(serviceUri, _requestContextAccessor);
        }

        public Task<IServiceHttpClient> CreateAsync(ServiceType service, CancellationToken cancellationToken)
        {
            return CreateAsync(service, null, cancellationToken);
        }

        public async Task<IEnumerable<IServiceHttpClient>> CreateAsync(IEnumerable<ServiceType> serviceTypes, CancellationToken cancellationToken)
        {
            IEnumerable<Uri> serviceUris = await _serviceLocator.LocateServicesAsync(serviceTypes, cancellationToken);
            return serviceUris.Select(uri => new ServiceHttpClient(uri, _requestContextAccessor)).ToArray();
        }
    }
}
