using AthliQ.Core.DTOs.Auth;
using AthliQ.Core.Service.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AthliQ.User.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _userService.RegisterAsync(registerDto);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpGet("ViewProfile")]
        public async Task<ActionResult> ViewProfile(string userId)
        {
            var result = await _userService.ViewProfile(userId);
            return Ok(result);
        }
    }
}
