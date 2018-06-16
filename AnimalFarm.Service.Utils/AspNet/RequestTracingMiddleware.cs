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

        public async Task InvokeAsync(HttpContext context, ServiceEventSource eventTrace)
        {
            string requestName = null;

            try
            {
                if (context.Request.Headers.TryGetValue(HeaderName.RequestId, out StringValues requestIds))
                {
                    string requestId = requestIds[0];
                    EventSource.SetCurrentThreadActivityId(Guid.Parse(requestId));
                    requestName = $"{context.Request.Path} - {requestId}";
                    eventTrace.ServiceRequestStart(requestName);
                }
            }
            catch (Exception)
            { }

            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceRequestStop(requestName, e.Message, e.StackTrace);
                throw;
            }

            if (requestName != null)
                ServiceEventSource.Current.ServiceRequestStop(requestName);
        }
    }
}
