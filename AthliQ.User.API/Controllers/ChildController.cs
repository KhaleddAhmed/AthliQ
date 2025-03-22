using System.Security.Claims;
using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Service.Contract;
using AthliQ.User.API.Attrbiutes;
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

        [HttpGet("ViewAllChildren")]
        [Authorize(Roles = "User")]
        [Cache(1)]
        public async Task<ActionResult> ViewAll(
            string? search,
            int? pageSize = 6,
            int? pageIndex = 1
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _childService.ViewAllChildrenAsync(
                userId,
                search,
                pageSize,
                pageIndex
            );
            return Ok(result);
        }

        [HttpGet("ViewChildDetails")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> ViewById(int childId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var child = await _childService.ViewChildAsync(childId);
            return Ok(child);
        }

        [HttpDelete("DeleteChild")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Delete(int childId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _childService.DeleteChildAsync(childId, userId);
            return Ok(result);
        }

        [HttpGet("EvaluteChildResult")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Evaluate(int childId)
        {
            var result = await _childService.EvaluateDataAsync(childId);
            return Ok(result);
        }
        #region Old One

        //[HttpPost("EvaluteChildResult")]
        //public async Task<ActionResult> Evaluate([FromBody]ChildToSendDto Player)
        //{
        //    var result = await _childService.Eva(Player);
        //    return Ok(result);
        //}
        #endregion
    }
}
