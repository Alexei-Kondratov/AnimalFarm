using AnimalFarm.Data;
using AnimalFarm.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AnimalFarm.RulesetService.Controllers
{
    [Route("")]
    public class RulesetController : Controller
    {
        private ITransactionManager _transactionManager;
        private IRepository<Ruleset> _rulesets;

        public RulesetController(ITransactionManager transactionManager, IRepository<Ruleset> rulesets)
        {
            _transactionManager = transactionManager;
            _rulesets = rulesets;
        }

        [HttpGet("{rulesetId}")]
        public async Task<IActionResult> GetRuleset(string rulesetId = null)
        {
            rulesetId = rulesetId ?? "BaseRuleset";

            using (var tx = _transactionManager.CreateTransaction())
            {
                var ruleset = await _rulesets.ByIdAsync(tx, rulesetId, rulesetId);
                await tx.CommitAsync();
                return Json(ruleset);
            }
        }
    }
}
