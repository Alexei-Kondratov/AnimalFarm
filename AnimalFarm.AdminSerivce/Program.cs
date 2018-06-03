using AnimalFarm.Service.Utils.Tracing;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;
using System.Threading;

namespace AnimalFarm.AdminService
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
                ServiceEventSource.SetName("AnimalFarm.Server-AnimalFarm.AdminServiceType");

                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("AnimalFarm.AdminServiceType",
                    context => new AdminService(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(AdminService).Name);

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
