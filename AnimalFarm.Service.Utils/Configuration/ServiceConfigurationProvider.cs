using AnimalFarm.Utils.Configuration;
using System.Fabric;

namespace AnimalFarm.Service.Utils.Configuration
{
    public class ServiceConfigurationProvider : IConfigurationProvider
    {
        private readonly ServiceContext _serviceContext;

        public ServiceConfigurationProvider(ServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
        }

        public TConfiguration GetConfiguration<TConfiguration>(string configurationName = null)
        {
            throw new System.NotImplementedException();
        }

        public string GetConnectionString(string connectionStringName = null)
        {
            connectionStringName = connectionStringName ?? "Default";
            return _serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["ConnectionStrings"].Parameters[connectionStringName].Value;
        }
    }
}
