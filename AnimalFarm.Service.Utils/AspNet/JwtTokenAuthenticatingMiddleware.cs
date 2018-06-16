using AnimalFarm.Service.Utils.Communication;
using AnimalFarm.Utils.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.AspNet
{
    public class JwtTokenAuthenticatingMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenAuthenticatingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private static void AuthenticateUserToken(HttpRequest request, JwtManager jwtManager)
        {
            string userToken = request.Headers[HeaderName.UserToken];

            if (String.IsNullOrEmpty(userToken))
                return;

            var userId = jwtManager.ValidateToken(userToken);

            if (userId != null)
                request.Headers.Add(HeaderName.UserId, userId);
        }

        public async Task InvokeAsync(HttpContext context, JwtManager jwtManager)
        {
            AuthenticateUserToken(context.Request, jwtManager);
            await _next.Invoke(context);
        }
    }
}
