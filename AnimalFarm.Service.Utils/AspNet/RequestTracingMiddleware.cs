using AnimalFarm.Service.Utils.Communication;
using AnimalFarm.Service.Utils.Tracing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.AspNet
{
    public class RequestTracingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTracingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ServiceLogger logger)
        {
            string requestName = "<Undefined>";
            string requestId = "<Undefined>";

            try
            {
                if (context.Request.Headers.TryGetValue(HeaderName.RequestId, out StringValues requestIds))
                {
                    requestId = requestIds[0];
                    EventSource.SetCurrentThreadActivityId(Guid.Parse(requestId));
                }

                logger.LogRequestStart(context.Request.Path, requestId);
            }
            catch (Exception)
            { }

            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                logger.LogRequestStop(context.Request.Path, requestId, e);
                throw;
            }

            if (requestName != null)
                logger.LogRequestStop(context.Request.Path, requestId);
        }
    }
}
