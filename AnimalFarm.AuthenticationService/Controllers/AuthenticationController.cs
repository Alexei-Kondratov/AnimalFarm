
using AnimalFarm.AuthenticationService.Messages;
using AnimalFarm.Data;
using AnimalFarm.Model;
using AnimalFarm.Service.Utils.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.AuthenticationService.Controllers
{
    [Route("")]
    public class AuthenticationController : Controller
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IRepository<UserAuthenticationInfo> _users;
        private readonly JwtManager _jwtManager;
        private readonly PasswordHasher _passwordHasher;

        public AuthenticationController(ITransactionManager transactionManager, IRepository<UserAuthenticationInfo> users)
        {
            _transactionManager = transactionManager;
            _users = users;
            _jwtManager = new JwtManager();
            _passwordHasher = new PasswordHasher();
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginMessage loginMessage)
        {
            var invalidLoginOrPasswordResult = BadRequest("Invalid login or password");

            if (loginMessage == null || String.IsNullOrEmpty(loginMessage.Login) || String.IsNullOrEmpty(loginMessage.Password))
                return invalidLoginOrPasswordResult;

            UserAuthenticationInfo userAuthenticationInfo;
            using (var tx = _transactionManager.CreateTransaction())
            {
                userAuthenticationInfo = await _users.ByIdAsync(tx, loginMessage.Login, loginMessage.Login);
            }

            if (userAuthenticationInfo == null)
                return invalidLoginOrPasswordResult;

            string receivedHash = _passwordHasher.GetHash(userAuthenticationInfo.PasswordSalt, loginMessage.Password);
            if (receivedHash != userAuthenticationInfo.PasswordHash)
                return invalidLoginOrPasswordResult;

            string token = _jwtManager.GenerateToken(userAuthenticationInfo.Id);
            return Ok(token);
        }
    }
}
