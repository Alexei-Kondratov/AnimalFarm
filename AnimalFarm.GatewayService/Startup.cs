﻿using System.Threading.Tasks;
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
            // TODO: Move to configuration

            builder
               .MapPost("admin/ClearCache", (context) => ForwardToAsync(ServiceType.Admin, "ClearCache", true))
               .MapPost("admin/ResetData", (context) => ForwardToAsync(ServiceType.Admin, "ResetData", true))
               .MapGet("animal/{id:guid}", (context) => ForwardToAsync(ServiceType.Animal, "{userId}/{id}"))
               .MapPut("animal/event", (context) => ForwardToAsync(ServiceType.Animal, "event"))
               .MapPost("login", (context) => ForwardToAsync(ServiceType.Authentication, "login", true))
               .MapGet("ruleset/{id:guid?}", (context) => ForwardToAsync(ServiceType.Ruleset, "{id}", true))
               .MapGet("updatesPlan", (context) => ForwardToAsync(ServiceType.Ruleset, "UpdatesPlan", true));
        }

        private Task ForwardToAsync(ServiceType service, string path, bool allowAnonymous = false)
        {
            return _requestForwarder.ForwardToAsync(service, path, allowAnonymous);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<GenerateRequestIdMiddleware>()
                .UseMiddleware<JwtTokenAuthenticatingMiddleware>()
                .UseMiddleware<RequestTracingMiddleware>()
                .UseMiddleware<CorsApplyingMiddleware>();

            app.UseRouter(BuildRoutes);
        }
    }
}
