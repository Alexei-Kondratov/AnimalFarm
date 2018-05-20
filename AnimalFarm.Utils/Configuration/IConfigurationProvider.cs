
namespace AnimalFarm.Utils.Configuration
{
    public interface IConfigurationProvider
    {
        string GetConnectionString(string connectionStringName = null);
        TConfiguration GetConfiguration<TConfiguration>(string configurationName = null);
    }
}
