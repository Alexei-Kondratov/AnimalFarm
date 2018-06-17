using AnimalFarm.Service.Utils.Communication;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.AspNet
{
    /// <summary>
    /// ASP NET middleware responsible for generating ids for incoming requests.
    /// </summary>
    public class GenerateRequestIdMiddleware
    {
        private readonly RequestDelegate _next;

        public GenerateRequestIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private void AddRequestId(HttpContext context)
        {
            var headers = context.Request.Headers;

            if (headers.ContainsKey(HeaderName.RequestId))
                headers.Remove(HeaderName.RequestId);

            headers.Add(HeaderName.RequestId, Guid.NewGuid().ToString());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            AddRequestId(context);
            await _next.Invoke(context);
        }
    }
}
