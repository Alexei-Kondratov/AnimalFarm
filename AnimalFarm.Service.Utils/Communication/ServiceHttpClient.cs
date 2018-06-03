using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Communication
{
    public class ServiceHttpClient : IServiceHttpClient
    {
        private readonly HttpClient _client;

        public ServiceHttpClient(Uri serviceAddress)
        {
            _client = new HttpClient(new HttpClientHandler(), true);
            _client.BaseAddress = serviceAddress;
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

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            return await _client.SendAsync(message, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }


    }
}
