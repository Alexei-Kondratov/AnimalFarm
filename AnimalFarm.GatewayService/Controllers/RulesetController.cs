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
            var internalClient = new ServiceHttpClient(ServiceType.Ruleset, null);
            var currentRuleset = await internalClient.GetAsync<Ruleset>("");
            return Json(currentRuleset);
        }
    }
}
