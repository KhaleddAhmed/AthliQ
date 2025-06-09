using AthliQ.Core.DTOs.Auth;
using AthliQ.Core.DTOs.User;
using AthliQ.Core.Responses;

namespace AthliQ.Core.Service.Contract
{
    public interface IUserService
    {
        Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto);
        Task<GenericResponse<UserDto>> RegisterAsync(RegisterDto registerDto);

        Task<bool> CheckEmailExistAsync(string email);

        Task<GenericResponse<ViewUserProfileDto>> ViewProfile(string userId);
    }
}
