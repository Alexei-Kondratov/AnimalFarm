using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.Communication;
using AnimalFarm.Service.Utils.Operations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.GatewayService.Controllers
{
    [Route("")]
    public class AdminController : Controller
    {
        private OperationRunner _operationRunner;
        private IServiceHttpClientFactory _serviceClientFactory;

        public AdminController(OperationRunner operationRunner, IServiceHttpClientFactory serviceClientFactory)
        {
            _operationRunner = operationRunner;
            _serviceClientFactory = serviceClientFactory;
        }

        [HttpPost("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            CancellationToken cancellationToken = CancellationToken.None;

            IEnumerable<IServiceHttpClient> clients =  await _serviceClientFactory.CreateAsync(new[] { ServiceType.Animal, ServiceType.Ruleset }, cancellationToken);

            Task sendClearCache(IServiceHttpClient client) 
                => client.SendAsync(new HttpRequestMessage(HttpMethod.Post, new Uri("admin/ClearCache", UriKind.Relative)), cancellationToken);

            IEnumerable<Task> operations = clients.Select(client => _operationRunner.RunAsync((context) => sendClearCache(client), cancellationToken));
            await Task.WhenAll(operations);
            return Ok();
        }
    }
}