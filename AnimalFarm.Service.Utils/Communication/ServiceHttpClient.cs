using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public class ServiceHttpClient : IServiceHttpClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceHttpClient(Uri serviceAddress)
        {
            _client = new HttpClient(new HttpClientHandler(), true);
            _client.BaseAddress = serviceAddress;
        }

        public ServiceHttpClient(Uri serviceAddress, IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient(new HttpClientHandler(), true);
            _client.BaseAddress = serviceAddress;
            _httpContextAccessor = httpContextAccessor;
        }
        //public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, HttpContent content, CancellationToken cancellationToken)
        //{
        //    var request = new HttpRequestMessage
        //    {
        //        RequestUri = new Uri(path),
        //        Method = method,
        //        Content = content
        //    };

        //    return await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
        //}

        private void PropagateRequestId(HttpRequestMessage message)
        {
            if (message.Headers.Contains(Headers.RequestId) || _httpContextAccessor == null)
                return;

            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(Headers.RequestId, out StringValues requestIds))
            {
                message.Headers.Add(Headers.RequestId, requestIds[0]);
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
