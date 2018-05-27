using Newtonsoft.Json;
using System.Collections.Generic;

namespace AnimalFarm.Model
{
    public class VersionSchedule : IHavePartition<string, string>
    {
        public string PartitionKey => BranchId;

        [JsonProperty(PropertyName = "id")]
        public string Id { get => BranchId; set => BranchId = value; }

        public string BranchId { get; set; }

        public IEnumerable<VersionScheduleRecord> Records { get; set; }
    }
}
