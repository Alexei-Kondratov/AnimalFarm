using System;

namespace AnimalFarm.Data.DataSources.Configuration
{
    public class DocumentDbConnectionInfo
    {
        public string Key { get; set; }
        public Uri Uri { get; set; }
        public string DatabaseName { get; set; }
    }
}
