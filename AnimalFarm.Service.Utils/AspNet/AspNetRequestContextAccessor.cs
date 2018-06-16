using AnimalFarm.Service.Utils.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace AnimalFarm.Service.Utils.AspNet
{
    public class AspNetRequestContextAccessor : IRequestContextAccessor
    {
        private IHttpContextAccessor _httpContextAccessor;

        public AspNetRequestContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetHeaderValue(string headerName)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(headerName, out StringValues requestIds))
                return requestIds[0];

            return null;
        }

        private RequestContext CreateRequestContext()
        {
            var requestId = GetHeaderValue(HeaderName.RequestId);
            var userId = GetHeaderValue(HeaderName.UserId);
            var isExternal = !String.IsNullOrEmpty(GetHeaderValue(HeaderName.IsExternal));
            return new RequestContext(requestId, userId, isExternal);
        }

        public RequestContext Context => CreateRequestContext();
    }
}
