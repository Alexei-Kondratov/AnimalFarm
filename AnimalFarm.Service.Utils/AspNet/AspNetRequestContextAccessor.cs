using AnimalFarm.Service.Utils.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace AnimalFarm.Service.Utils.AspNet
{
    /// <summary>
    /// Implements IRequestContextAccessor as a wrapper around ASP NET HttpContext.
    /// </summary>
    public class AspNetRequestContextAccessor : IRequestContextAccessor
    {
        private IHttpContextAccessor _httpContextAccessor;

        public AspNetRequestContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region Interface implementation IRequestContextAccessor

        public RequestContext Context => CreateRequestContext();

        #endregion Interface implementation IRequestContextAccessor

        #region Private methods

        private string GetHeaderValue(HttpRequest request, string headerName)
        {
            if (request.Headers.TryGetValue(headerName, out StringValues requestIds))
                return requestIds[0];

            return null;
        }

        private RequestContext CreateRequestContext()
        {
            HttpRequest request = _httpContextAccessor?.HttpContext?.Request;

            if (request == null)
                return null;

            var requestId = GetHeaderValue(request, HeaderName.RequestId);
            var userId = GetHeaderValue(request, HeaderName.UserId);
            var isExternal = !String.IsNullOrEmpty(GetHeaderValue(request, HeaderName.IsExternal));
            return new RequestContext(requestId, userId, isExternal);
        }

        #endregion Private methods
    }
}
