using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Service;
using AnimalFarm.Services.Utils.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.AnimalService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class AnimalService : BaseStatefulService
    {
        public AnimalService(StatefulServiceContext context)
            : base(context)
        {
        }

        protected override void SetupWebHost(IWebHostBuilder builder)
        {
            base.SetupWebHost(builder);
            builder.UseStartup<Startup>();
        }

        protected override void RegisterServices(IServiceCollection serviceCollection)
        {
            base.RegisterServices(serviceCollection);

            serviceCollection
                .AddSingleton<RulesetScheduleProvider>()
                .AddRepository<Animal>()
                .AddRepository<Ruleset>()
                .AddRepository<VersionSchedule>();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne();
        }
    }
}
