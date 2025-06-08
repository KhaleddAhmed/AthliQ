using AthliQ.Core.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AthliQ.User.API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPut("AcceptUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Accept(string userId)
        {
            var user = await _adminService.AcceptUserAsync(userId);
            return Ok(user);
        }

        [HttpPut("RejectUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Reject(string userId)
        {
            var user = await _adminService.RejectUserAsync(userId);
            return Ok(user);
        }

        [HttpPut("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string userId)
        {
            var user = await _adminService.DeleteUserAsync(userId);
            return Ok(user);
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Get(int? pageIndex = 1, int? pageSize = 6)
        {
            var result = await _adminService.GetAllUsersAsync(pageIndex, pageSize);
            return Ok(result);
        }
    }
}
