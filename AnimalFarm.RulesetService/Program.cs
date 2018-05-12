using AnimalFarm.Data;
using AnimalFarm.Model;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.RulesetService
{
    internal static class Program
    {
        private static IRepository<Ruleset> BuildRulesetRepository()
        {
            return new AzureTableRepository<Ruleset>(
                       "DefaultEndpointsProtocol=https;AccountName=animalfarm;AccountKey=7Lrjq5wId8TCpSx5o7vFI4nxVugkhjZcOh25RCSp318HIeXDE4o8SkaoVgeb5vKnNtrGXkJapS+Mmuf0Tnp7GA==;EndpointSuffix=core.windows.net",
                       "Rulesets",
                       "Rules");
        }

        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("AnimalFarm.RulesetServiceType",
                    context => new RulesetService(context, BuildRulesetRepository())).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(RulesetService).Name);

                // Prevents this host process from terminating so services keeps running. 
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
