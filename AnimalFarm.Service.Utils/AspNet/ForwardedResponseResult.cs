using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace AnimalFarm.Service.Utils.AspNet
{
    public class ForwardedResponseResult : ContentResult
    {
        public ForwardedResponseResult(HttpResponseMessage response)
            : base() 
        {
            StatusCode = (int)response.StatusCode;
            Content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            ContentType = response.Content.Headers.ContentType?.ToString();
        }
    }
}
