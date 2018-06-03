using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AnimalFarm.Model
{
    public class UserAuthenticationInfo : TableEntity, IHavePartition<string, string>
    {
        private string _login;

        [JsonProperty(PropertyName = "id")]
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                PartitionKey = value;
                RowKey = value;
            }
        }

        public string Id { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
    }
}
