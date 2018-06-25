using AnimalFarm.Model;
using AnimalFarm.Utils.Configuration;
using Newtonsoft.Json;

namespace AnimalFarm.Service.Utils.Configuration
{
    public class ConfigurationRecord : IHavePartition<string, string>
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string PartitionKey { get; set; }
        public string SerializedConfiguration { get; set; }

        public ConfigurationRecord()
        {
        }

        public ConfigurationRecord(string fullName, object configurationObj)
        {
            Id = fullName;
            PartitionKey = fullName;
            SerializedConfiguration = JsonConvert.SerializeObject(configurationObj, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple });
        }

        public ConfigurationRecord(string @namespace, IComponentConfiguration componentConfiguration)
            : this(@namespace, componentConfiguration.Key, componentConfiguration)
        {
        }

        public ConfigurationRecord(string @namespace, string name, object configurationObj)
             : this($"{@namespace}-{name}", configurationObj)
        {
        }
    }
}
