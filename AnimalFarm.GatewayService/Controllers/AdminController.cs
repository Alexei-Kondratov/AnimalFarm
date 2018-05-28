using AnimalFarm.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalFarm.GatewayService.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        [HttpPost("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            var internalClient = new ServiceHttpClient(ServiceType.Ruleset, "");
            var currentRuleset = await internalClient.SendAsync(HttpMethod.Post, "admin/ClearCache", null);
            return Ok();
        }
    }
}