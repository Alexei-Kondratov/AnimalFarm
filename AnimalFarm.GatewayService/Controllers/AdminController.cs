﻿using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.AspNet;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AnimalFarm.GatewayService.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        [HttpPost("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            var response = await new ServiceHttpClient(ServiceType.Admin, "").ForwardAsync(Request, "ClearCache");
            return new ForwardedResponseResult(response);
        }
    }
}