using AnimalFarm.Data.Seed;
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

        public AdminController(OperationRunner operationRunner, IServiceHttpClientFactory serviceClientFactory)
        {
            _operationRunner = operationRunner;
        }

        private static async Task ClearCacheAsync(OperationContext context, IServiceHttpClientFactory serviceClientFactory)
        {
            IEnumerable<IServiceHttpClient> clients
                = await serviceClientFactory.CreateAsync(new[] { ServiceType.Animal, ServiceType.Ruleset }, context.CancellationToken);

            Task sendClearCache(OperationContext ctx, IServiceHttpClient client)
                => client.SendAsync(new HttpRequestMessage(HttpMethod.Post, new Uri("admin/ClearCache", UriKind.Relative)), ctx.CancellationToken);

            IEnumerable<Task> runSendClearCacheTasks = clients.Select(c => context.RunSuboperationAync((ctx) => sendClearCache(ctx, c)));
            await Task.WhenAll(runSendClearCacheTasks);
        }

        private static async Task ResetDataAsync(OperationContext context, IDataSeeder dataSeeder, SeedData seedData, IServiceHttpClientFactory serviceClientFactory)
        {
            await dataSeeder.SeedAsync(seedData);
            await ClearCacheAsync(context, serviceClientFactory);
        }

        [HttpPost("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            await _operationRunner.RunAsync<IServiceHttpClientFactory>(ClearCacheAsync, CancellationToken.None);
            return Ok();
        }

        [HttpPost("ResetData")]
        public async Task<IActionResult> ResetData()
        {
            await _operationRunner.RunAsync<IDataSeeder, SeedData, IServiceHttpClientFactory>(ResetDataAsync, CancellationToken.None);
            return Ok();
        }
    }
}