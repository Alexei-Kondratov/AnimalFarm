using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.GatewayService
{
    public class RequestForwarder
    {
        private readonly IServiceHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IRequestContextAccessor _requestContextAccessor;

        public RequestForwarder(IServiceHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IRequestContextAccessor requestContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _requestContextAccessor = requestContextAccessor;
        }

        private string FillRouteValues(string path, RouteValueDictionary routeValues)
        {
            var regex = @"{(.*?)}";
            var result = path;
            foreach (Match match in Regex.Matches(path, regex))
            {
                string parameterName = match.Value.Substring(1, match.Value.Length - 2);
                string parameterValue = routeValues.ContainsKey(parameterName) ? routeValues[parameterName].ToString() : String.Empty;
                result = result.Replace(match.Value, parameterValue);
            }

            return result;
        }

        private HttpRequestMessage BuildRequest(HttpRequest externalRequest, string path)
        {
            var result = new HttpRequestMessage
            {
                RequestUri = new Uri(path, UriKind.Relative),
                Method = new HttpMethod(externalRequest.Method),
            };

            if (result.Method == HttpMethod.Post || result.Method == HttpMethod.Put)
                result.Content = new StreamContent(externalRequest.Body);

            if (externalRequest.ContentType != null)
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(externalRequest.ContentType.Split(';')[0]);

            if (_requestContextAccessor.Context.RequestId != null)
                result.Headers.Add(HeaderName.RequestId, _requestContextAccessor.Context.RequestId);

            if (_requestContextAccessor.Context.UserId != null)
                result.Headers.Add(HeaderName.UserId, _requestContextAccessor.Context.UserId);

            return result;
        }

        private async Task WriteResponseAsync(HttpResponseMessage response, HttpResponse externalResponse)
        {
            externalResponse.StatusCode = (int)response.StatusCode;
            if (response.Content != null)
                await externalResponse.WriteAsync(await response.Content.ReadAsStringAsync());
        }

        public async Task ForwardToAsync(ServiceType serviceType, string path, bool allowAnonymous = false)
        {
            HttpContext context = _httpContextAccessor.HttpContext;

            string userId = _requestContextAccessor.Context.UserId;
            if (!allowAnonymous && userId == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            if (userId != null)
                context.GetRouteData().Values.Add(nameof(userId), userId);

            path = FillRouteValues(path, context.GetRouteData().Values);
            HttpRequestMessage fwRequest = BuildRequest(context.Request, path);

            HttpResponseMessage response;
            using (IServiceHttpClient client = await _httpClientFactory.CreateAsync(serviceType, userId, CancellationToken.None))
            {
                response = await client.SendAsync(fwRequest, CancellationToken.None);
            }

            await WriteResponseAsync(response, context.Response);
        }
    }
}
