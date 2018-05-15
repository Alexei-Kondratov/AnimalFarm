using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.Model
{
    public class UserAuthenticationInfo : TableEntity
    {
        private string _login;

        public string Login
        {
            get => _login;
            set
            {
                Id = value;
                PartitionKey = value;
                RowKey = value;
            }
        }

        public string Id { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
    }
}
