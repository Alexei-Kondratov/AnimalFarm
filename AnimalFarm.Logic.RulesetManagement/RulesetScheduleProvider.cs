using AnimalFarm.Data;
using AnimalFarm.Model;
using AnimalFarm.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.RulesetManagement
{
    public class RulesetScheduleProvider
    {
        private readonly string _branchId;
        private readonly IRepository<VersionSchedule> _scheduleRepository;

        public RulesetScheduleProvider(IConfigurationProvider configurationProvider, IRepository<VersionSchedule> scheduleRepository)
            : this(configurationProvider.GetConfigurationAsync<BranchConfiguration>().GetAwaiter().GetResult().ActiveBranchId, scheduleRepository)
        {
        }

        public RulesetScheduleProvider(string branchId, IRepository<VersionSchedule> scheduleRepository)
        {
            _branchId = branchId;
            _scheduleRepository = scheduleRepository;
        }

        private async Task<IEnumerable<VersionScheduleRecord>> GetRecords(ITransaction transaction)
        {
            VersionSchedule schedule = await _scheduleRepository.ByIdAsync(transaction, _branchId, _branchId);
            return schedule.Records.OrderByDescending(r => r.Start);
        }

        public async Task<VersionScheduleRecord> GetActiveRulesetRecordAsync(ITransaction transaction, DateTime time)
        {
            IEnumerable<VersionScheduleRecord> records = await GetRecords(transaction);
            return records.First(r => r.Start <= time);
        }

        public async Task<IDictionary<DateTime, string>> GetActiveRulesetRecordsAsync(ITransaction transaction, DateTime start, DateTime end)
        {
            IEnumerable<VersionScheduleRecord> records = await GetRecords(transaction);

            var result = records.SkipWhile(r => r.Start > end).TakeWhile(r => r.Start > start);
            return result.ToDictionary(r => r.Start, r => r.RulesetId);
        }
    }
}
