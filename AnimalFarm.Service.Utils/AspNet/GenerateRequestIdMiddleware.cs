using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.AspNet
{
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

            if (headers.ContainsKey("Request-Id"))
                headers.Remove("Request-Id");

            headers.Add("Request-Id", Guid.NewGuid().ToString());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            AddRequestId(context);
            await _next.Invoke(context);
        }
    }
}
