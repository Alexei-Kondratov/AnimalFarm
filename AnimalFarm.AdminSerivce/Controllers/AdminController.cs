using AnimalFarm.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalFarm.GatewayService.Controllers
{
    [Route("")]
    public class AdminController : Controller
    {
        [HttpPost("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            await new ServiceHttpClient(ServiceType.Ruleset, "").BroadcastAsync(new[] { ServiceType.Animal, ServiceType.Ruleset }, HttpMethod.Post, "admin/ClearCache", null);
            return Ok();
        }
    }
}