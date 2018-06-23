using AnimalFarm.Data;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalFarm.RulesetService.Controllers
{
    [Route("")]
    public class RulesetController : Controller
    {
        private IRepository<Ruleset> _rulesets;
        private RulesetScheduleProvider _scheduleProvider;
        private ITransactionManager _transactionManager;

        public RulesetController(ITransactionManager transactionManager, RulesetScheduleProvider scheduleProvider, IRepository<Ruleset> rulesets)
        {
            _rulesets = rulesets;
            _scheduleProvider = scheduleProvider;
            _transactionManager = transactionManager;
             
        }

        [HttpGet("")]
        [HttpGet("{rulesetId}")]
        [HttpGet("Rulesets/{rulesetId}")]
        public async Task<IActionResult> GetRuleset(string rulesetId = null)
        {
            using (var tx = _transactionManager.CreateTransaction())
            {
                rulesetId = rulesetId ?? (await _scheduleProvider.GetActiveRulesetRecordAsync(tx, DateTime.UtcNow)).RulesetId;
                var ruleset = await _rulesets.ByIdAsync(tx, rulesetId, rulesetId);
                await tx.CommitAsync();
                return Json(ruleset);
            }
        }

        [HttpGet("UpdatesPlan")]
        public async Task<IActionResult> GetPlannedUpdates()
        {
            using (var tx = _transactionManager.CreateTransaction())
            {
                IDictionary<DateTime, string> plannedRulesetRecords =
                    await _scheduleProvider.GetActiveRulesetRecordsAsync(tx, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

                return Json(plannedRulesetRecords);
            }
        }
    }
}
