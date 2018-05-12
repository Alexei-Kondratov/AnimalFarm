using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AnimalFarm.GatewayService.Controllers
{
    [Route("ruleset")]
    public class RulesetController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var internalClient = new InternalHttpClient();
            var currentRuleset = await internalClient.GetAsync<Ruleset>(InternalService.Ruleset, "");
            return Json(currentRuleset);
        }
    }
}
