﻿using AnimalFarm.Data;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Service;
using AnimalFarm.Service.Utils.Operations;
using AnimalFarm.Services.Utils.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.RulesetService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class RulesetService : BaseStatefulService
    {
        public RulesetService(StatefulServiceContext context)
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
                .AddSingleton<OperationRunner>()
                .AddSingleton<RulesetScheduleProvider>()
                .AddSingleton<RulesetUnpacker>()
                .AddSingleton<RulesetUnpackingDecorator>()
                .AddRepository<Ruleset>()
                .AddRepository<VersionSchedule>();
        }

        private async Task PreloadCurrentRuleset(OperationContext context, RulesetScheduleProvider scheduleProvider, IRepository<Ruleset> rulesets)
        {
            var rulesetId = (await scheduleProvider.GetActiveRulesetRecordAsync(context.Transaction, DateTime.UtcNow)).RulesetId;
            var ruleset = await rulesets.ByIdAsync(context.Transaction, rulesetId, rulesetId);
            context.Logger.Log($"Preloaded ruleset '{ruleset.Id}'");
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var operationRunner = ServiceProvider.GetRequiredService<OperationRunner>();
            await operationRunner.RunAsync<RulesetScheduleProvider, IRepository<Ruleset>>(PreloadCurrentRuleset, cancellationToken);
            cancellationToken.WaitHandle.WaitOne();
        }
    }
}
