using System.Threading.Tasks;
using AnimalFarm.Data.Cache;
using Microsoft.AspNetCore.Mvc;

namespace AnimalFarm.RulesetService.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private CacheManager _cacheManager;

        public AdminController(CacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        [HttpPost("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            await _cacheManager.ClearAllAsync();
            return Ok();
        }
    }
}