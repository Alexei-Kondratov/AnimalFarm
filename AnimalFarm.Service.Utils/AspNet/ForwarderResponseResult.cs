using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AnimalFarm.Service.Utils.AspNet
{
    public class ForwarderResponseResult : ContentResult
    {
        public ForwarderResponseResult(HttpResponseMessage response)
            : base() 
        {
            StatusCode = (int)response.StatusCode;
            Content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            ContentType = response.Content.Headers.ContentType?.ToString();
        }
    }
}
