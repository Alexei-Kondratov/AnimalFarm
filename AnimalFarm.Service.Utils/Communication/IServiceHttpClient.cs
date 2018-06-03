using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public interface IServiceHttpClient : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken);
    }
}
