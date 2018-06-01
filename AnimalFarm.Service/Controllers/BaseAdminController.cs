using System.Threading.Tasks;
using AnimalFarm.Data.Cache;
using Microsoft.AspNetCore.Mvc;

namespace AnimalFarm.Service.Controllers
{
    [Route("admin")]
    public class BaseAdminController : Controller
    {
        private CacheManager _cacheManager;

        public BaseAdminController(CacheManager cacheManager)
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