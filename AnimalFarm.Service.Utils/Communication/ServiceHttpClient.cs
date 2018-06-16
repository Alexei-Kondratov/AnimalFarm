using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public class ServiceHttpClient : IServiceHttpClient
    {
        private readonly HttpClient _client;
        private readonly IRequestContextAccessor _requestContextAccessor;

        public ServiceHttpClient(Uri serviceAddress)
        {
            _client = new HttpClient(new HttpClientHandler(), true);
            _client.BaseAddress = serviceAddress;
        }

        public ServiceHttpClient(Uri serviceAddress, IRequestContextAccessor requestContextAccessor)
        {
            _client = new HttpClient(new HttpClientHandler(), true);
            _client.BaseAddress = serviceAddress;
            _requestContextAccessor = requestContextAccessor;
        }

        private void PropagateRequestId(HttpRequestMessage message)
        {
            if (message.Headers.Contains(HeaderName.RequestId) || _requestContextAccessor == null)
                return;

            if (_requestContextAccessor.Context.RequestId != null)
            {
                message.Headers.Add(HeaderName.RequestId, _requestContextAccessor.Context.RequestId);
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            PropagateRequestId(message);
            return await _client.SendAsync(message, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
