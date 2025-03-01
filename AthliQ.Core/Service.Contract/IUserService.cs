using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Auth;
using AthliQ.Core.Responses;

namespace AthliQ.Core.Service.Contract
{
    public interface IUserService
    {
        Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto);
        Task<GenericResponse<UserDto>> RegisterAsync(RegisterDto registerDto);

        Task<bool> CheckEmailExistAsync(string email);
    }
}
