using AnimalFarm.Service.Utils.Tracing;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;
using System.Threading;

namespace AnimalFarm.RulesetService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                ServiceEventSource.SetName("AnimalFarm.Server-AnimalFarm.RulesetServiceType");

                ServiceRuntime.RegisterServiceAsync("AnimalFarm.RulesetServiceType",
                    context => new RulesetService(context)).GetAwaiter().GetResult();

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
