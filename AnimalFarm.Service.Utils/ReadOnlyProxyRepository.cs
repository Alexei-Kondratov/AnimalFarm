using AnimalFarm.Model;
using System;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading.Tasks;
using AnimalFarm.Data;

namespace AnimalFarm.Service.Utils
{
    public class ReadOnlyProxyRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IHaveId<string>
    {
        private readonly ServiceType _serviceType; 
        private readonly string _endpointPath;

        public ReadOnlyProxyRepository(ServiceType serviceType, string endpointPath)
        {
            _serviceType = serviceType;
            _endpointPath = endpointPath;
        }

        public async Task<TEntity> ByIdAsync(ITransaction transaction, string partitionId, string entityId)
        {
            var client = new ServiceHttpClient(_serviceType, partitionId);
            var formattedPath = _endpointPath.Replace("{partitionId}", partitionId).Replace("{entityId}", entityId);
            return await client.GetAsync<TEntity>(formattedPath);
        }

        public Task UpsertAsync(ITransaction transaction, TEntity entity)
        {
            throw new NotSupportedException("ReadOnlyProxyRepository does not support write operations");
        }
    }
}
