using System.Security.Claims;
using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AthliQ.User.API.Controllers
{
    public class ChildController : BaseApiController
    {
        private readonly IChildService _childService;

        public ChildController(IChildService childService)
        {
            _childService = childService;
        }

        [HttpPost("CreateChild")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Create([FromForm] CreateChildDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _childService.CreateChildAsync(userId, dto);
            return Ok(result);
        }
    }
}
