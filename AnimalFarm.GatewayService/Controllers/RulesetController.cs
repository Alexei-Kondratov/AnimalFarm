using System.Threading.Tasks;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AnimalFarm.GatewayService.Controllers
{
    [Route("ruleset")]
    public class RulesetController : Controller
    {
        [HttpGet("")]
        [HttpGet("{rulesetId}")]
        public async Task<IActionResult> GetAsync(string rulesetId = "")
        {
            var internalClient = new ServiceHttpClient(ServiceType.Ruleset, "");
            var currentRuleset = await internalClient.GetAsync<Ruleset>($"{rulesetId}");
            return Json(new { Data = currentRuleset });
        }
    }
}
