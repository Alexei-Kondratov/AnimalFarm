using System.Threading.Tasks;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.AspNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;

namespace AnimalFarm.GatewayService
{
    public class Startup
    {
        private readonly RequestForwarder _requestForwarder;

        public Startup(RequestForwarder requestForwarder)
        {
            _requestForwarder = requestForwarder;
        }
       
        private void BuildRoutes(IRouteBuilder builder)
        {
            builder
               .MapPost("admin/ClearCache", (context) => ForwardToAsync(ServiceType.Admin, "ClearCache"))
               .MapPost("admin/ResetData", (context) => ForwardToAsync(ServiceType.Admin, "ResetData"))
               .MapGet("animal/{id:guid}", (context) => ForwardToAsync(ServiceType.Animal, "{id}"))
               .MapPut("animal/event", (context) => ForwardToAsync(ServiceType.Animal, "event"))
               .MapPost("login", (context) => ForwardToAsync(ServiceType.Authentication, "login"))
               .MapGet("ruleset/{id:guid?}", (context) => ForwardToAsync(ServiceType.Ruleset, "{id}"));
        }

        private Task ForwardToAsync(ServiceType service, string path)
        {
            return _requestForwarder.ForwardToAsync(service, path);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<GenerateRequestIdMiddleware>()
                .UseMiddleware<JwtTokenAuthenticatingMiddleware>()
                .UseMiddleware<RequestTracingMiddleware>();

            app.UseRouter(BuildRoutes);
        }
    }
}
