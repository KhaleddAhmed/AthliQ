using AthliQ.Core.DTOs.Test;
using AthliQ.Core.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AthliQ.User.API.Controllers
{
    
    public class TestController : BaseApiController
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateTest")]
        public async Task<ActionResult> Create([FromBody] CreateTestDto createTestDto)
        {
            var result = await _testService.CreateTestAsync(createTestDto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateTest")]
        public async Task<ActionResult> Update(UpdateTestDto updateTestDto)
        {
            var result = await _testService.UpdateTestAsync(updateTestDto);
            return Ok(result);
        }

        [HttpGet("GetAllTests")]
        public async Task<ActionResult> GetAllTests(int? categoryid)
        {
            var result = await _testService.GetAllTestAsync(categoryid);
            return Ok(result);
        }

        [HttpGet("GetTestDetails")]
        public async Task<ActionResult> GetTestById(int id)
        {
            var result = await _testService.GetTestAsync(id);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteTest")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _testService.DeleteTestAsync(id);
            return Ok(result);
        }
    }
}
