using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.AspNet
{
    /// <summary>
    /// ASP NET middleware responsible for applying a CORS (Cross-Origin Resource Sharing) policy.
    /// </summary>
    public class CorsApplyingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICorsService _corsService;

        public CorsApplyingMiddleware(RequestDelegate next, ICorsService corsService)
        {
            _corsService = corsService;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, CorsPolicy corsPolicy)
        {
            CorsResult evaluationResult = _corsService.EvaluatePolicy(context, corsPolicy);
            _corsService.ApplyResult(evaluationResult, context.Response);

            if (context.Request.Method == "OPTIONS")
                return;

            await _next.Invoke(context);
        }
    }
}
