using System.Collections.Generic;

namespace AnimalFarm.Model
{
    public class VersionSchedule
    {
        public string BranchId { get; set; }

        public IEnumerable<VersionScheduleRecord> Records { get; set; }
    }
}
