using System.Net.Http;
using System.Threading.Tasks;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.AspNet;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnimalFarm.GatewayService.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        [HttpPost]
        public async Task<object> Index()
        {
            var internalClient = new ServiceHttpClient(ServiceType.Authentication, "");
            var response =  await internalClient.ForwardAsync(Request, "login");
            return new ForwarderResponseResult(response);
        }
    }
}
