using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public interface IServiceHttpClientFactory
    {
        Task<IServiceHttpClient> CreateAsync(ServiceType service, CancellationToken cancellationToken);
        Task<IServiceHttpClient> CreateAsync(ServiceType service, string partitionKey, CancellationToken cancellationToken);
        Task<IEnumerable<IServiceHttpClient>> CreateAsync(IEnumerable<ServiceType> serviceTypes, CancellationToken cancellationToken);
    }
}
