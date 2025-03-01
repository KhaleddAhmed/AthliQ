using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Auth
{
    public class UserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }

        public string Token { get; set; }
    }
}
