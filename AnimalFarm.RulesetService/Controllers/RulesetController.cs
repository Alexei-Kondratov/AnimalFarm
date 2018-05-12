using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalFarm.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace AnimalFarm.RulesetService.Controllers
{
    [Route("")]
    public class RulesetController : Controller
    {
        private IReliableStateManager _stateManager;

        public RulesetController(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentRuleset()
        {
            using (var tx = _stateManager.CreateTransaction())
            {
                var rulesetDictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<string, Ruleset>>("Rulesets");
                var ruleset = await rulesetDictionary.TryGetValueAsync(tx, "BaseRuleset");
                return Json(ruleset.Value);
            }
        }
    }
}
