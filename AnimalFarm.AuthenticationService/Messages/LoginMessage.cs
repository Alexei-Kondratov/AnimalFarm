using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalFarm.AuthenticationService.Messages
{
    public class LoginMessage
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
