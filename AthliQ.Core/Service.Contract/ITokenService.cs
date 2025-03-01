using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace AthliQ.Core.Service.Contract
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AthliQUser user, UserManager<AthliQUser> userManager);
    }
}
