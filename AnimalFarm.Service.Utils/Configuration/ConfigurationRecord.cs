using AnimalFarm.Model;
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

        public ConfigurationRecord(string name, object configurationObj)
        {
            string typeName = configurationObj.GetType().Name;
            if (!string.IsNullOrEmpty(name))
                name = $"{typeName}-{name}";
            else
                name = $"{typeName}";

            Id = name;
            PartitionKey = name;
            SerializedConfiguration = JsonConvert.SerializeObject(configurationObj);
        }
    }
}
