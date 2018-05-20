using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using AnimalFarm.Service.Utils;
using AnimalFarm.Service.Utils.AspNet;
using AnimalFarm.Utils.Security;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalFarm.GatewayService.Controllers
{
    public class ExternalAnimalEvent
    {
        public string EventType { get; set; }
    }

    [Route("animal")]
    public class AnimalController : Controller
    {
        private JwtManager _jwtManager;

        public AnimalController(JwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        private Type GetInternalEventType(string type)
        {
            switch (type)
            {
                case "Create": return typeof(CreateAnimalEvent);
                case "Interact": return typeof(AnimalActionEvent);
                default: return null;
            }
        }

        private object DeserializeRequestBody(Type type)
        {
            Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
            var body = (new StreamReader(Request.Body)).ReadToEnd();
            return JsonConvert.DeserializeObject(body, type, new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            });
        }

        private string GetUserId()
        {
            string userToken = Request.Headers["User-Token"];

            if (String.IsNullOrEmpty(userToken))
                return null;

            return _jwtManager.ValidateToken(userToken);
        }

        [HttpGet("{animalId}")]
        public async Task<IActionResult> GetAsync(string animalId)
        {
            var userId = GetUserId();
            if (userId == null)
                return BadRequest("Invalid user token");

            var internalClient = new ServiceHttpClient(ServiceType.Animal, userId);
            var animal = await internalClient.GetAsync<Animal>($"{userId}/{animalId}");
            return Json(new { Data = animal });
        }

        [HttpPut("event")]
        public async Task<IActionResult> PushEvent([FromBody] ExternalAnimalEvent externalEvent)
        {
            var userId = GetUserId();
            if (userId == null)
                return BadRequest("Invalid user token");

            if (externalEvent == null)
                return BadRequest("Cannot read event data");

            Type internalEventType = GetInternalEventType(externalEvent.EventType);
            if (internalEventType == null)
                return BadRequest("Invalid event type");

            AnimalEvent internalEvent = (AnimalEvent)DeserializeRequestBody(internalEventType);

            internalEvent.ActingUserId = userId;
            var internalClient = new ServiceHttpClient(ServiceType.Animal, userId);
            var response = await internalClient.SendAsync(HttpMethod.Put, "event", internalEvent, typeof(AnimalEvent));
            return new ForwardedResponseResult(response);
        }
    }
}
