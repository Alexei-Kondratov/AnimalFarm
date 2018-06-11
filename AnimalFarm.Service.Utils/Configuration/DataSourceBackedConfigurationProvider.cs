using AnimalFarm.Data;
using AnimalFarm.Utils.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Configuration
{
    public class DataSourceBackedConfigurationProvider : IConfigurationProvider
    {
        private readonly IDataSource _dataSource;
        private readonly ITransactionManager _transactionManager;

        public DataSourceBackedConfigurationProvider(IDataSource dataSource, ITransactionManager transactionManager)
        {
            _dataSource = dataSource;
            _transactionManager = transactionManager;
        }

        public async Task<TConfiguration> GetConfigurationAsync<TConfiguration>(string configurationName = null)
        {
            string typeName = typeof(TConfiguration).Name;
            if (!string.IsNullOrEmpty(configurationName))
                configurationName = $"{typeName}-{configurationName}";
            else
                configurationName = $"{typeName}";

            ConfigurationRecord record;

            using (ITransaction transaction = _transactionManager.CreateTransaction()) {
                record = await _dataSource.ByIdAsync<ConfigurationRecord>(transaction, "Configuration", configurationName, configurationName);
            }

            return JsonConvert.DeserializeObject<TConfiguration>(record.SerializedConfiguration, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }
    }
}
