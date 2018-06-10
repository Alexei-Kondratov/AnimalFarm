using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.AspNet;
using AnimalFarm.Service.Utils.Communication;
using AnimalFarm.Utils.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalFarm.GatewayService
{
    public class Startup
    {
        private readonly JwtManager _jwtManager;
        private readonly IServiceHttpClientFactory _httpClientFactory;

        public Startup(JwtManager jwtManager, IServiceHttpClientFactory httpClientFactory)
        {
            _jwtManager = jwtManager;
            _httpClientFactory = httpClientFactory;
        }

        private string GetUserId(HttpRequest request)
        {
            string userToken = request.Headers["User-Token"];

            if (String.IsNullOrEmpty(userToken))
                return null;

            return _jwtManager.ValidateToken(userToken);
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

        // TODO: Extract the forwarding out of Startup. 
        private async Task ForwardToAsync(HttpContext context, ServiceType serviceType, string path)
        {
            string userId = GetUserId(context.Request);
            var client = await _httpClientFactory.CreateAsync(serviceType, userId, CancellationToken.None);

            path = FillRouteValues(path, context.GetRouteData().Values);

            var fwRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(path, UriKind.Relative),
                Method = new HttpMethod(context.Request.Method),
            };

            if (fwRequest.Method == HttpMethod.Post || fwRequest.Method == HttpMethod.Put)
                fwRequest.Content = new StreamContent(context.Request.Body);

            if (context.Request.ContentType != null)
                fwRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(context.Request.ContentType.Split(';')[0]);

            if (context.Request.Headers.ContainsKey("Request-Id"))
                fwRequest.Headers.Add("Request-Id", context.Request.Headers["Request-Id"][0]);

            if (userId != null)
                fwRequest.Headers.Add("User-Id", userId);

            HttpResponseMessage response = await client.SendAsync(fwRequest, CancellationToken.None);

            context.Response.StatusCode = (int)response.StatusCode;
            if (response.Content != null)
                await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
        }

        private void BuildRoutes(IRouteBuilder builder)
        {
            builder
               .MapPost("admin/ClearCache", (context) => ForwardToAsync(context, ServiceType.Admin, "ClearCache"))
               .MapPost("admin/ResetData", (context) => ForwardToAsync(context, ServiceType.Admin, "ResetData"))
               .MapGet("animal/{id:guid}", (context) => ForwardToAsync(context, ServiceType.Animal, "{id}"))
               .MapPut("animal/event", (context) => ForwardToAsync(context, ServiceType.Animal, "event"))
               .MapPost("login", (context) => ForwardToAsync(context, ServiceType.Authentication, "login"))
               .MapGet("ruleset/{id:guid?}", (context) => ForwardToAsync(context, ServiceType.Ruleset, "{id}"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<GenerateRequestIdMiddleware>()
                .UseMiddleware<RequestTracingMiddleware>();

            app.UseRouter(BuildRoutes);
        }
    }
}
