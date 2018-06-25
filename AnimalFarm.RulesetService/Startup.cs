using AnimalFarm.Service.Utils.AspNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AnimalFarm.RulesetService
{
    public class MyServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            throw new NotImplementedException();
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestTracingMiddleware>();
            app.UseMvc();
        }
    }
}
