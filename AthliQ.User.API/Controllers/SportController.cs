using AthliQ.Core.DTOs.Sport;
using AthliQ.Core.Service.Contract;
using AthliQ.User.API.Attrbiutes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AthliQ.User.API.Controllers
{
    public class SportController : BaseApiController
    {
        private readonly ISportService _sportService;

        public SportController(ISportService sportService)
        {
            _sportService = sportService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateSport")]
        public async Task<IActionResult> Create([FromBody] CreateSportDto createSportDto)
        {
            var sport = await _sportService.CreateSportAsync(createSportDto);
            return Ok(sport);
        }

        [HttpGet("GetSport")]
        public async Task<IActionResult> GetById(int id)
        {
            var sport = await _sportService.GetSportAsync(id);
            return Ok(sport);
        }

        [HttpGet("GetAllSports")]
        public async Task<IActionResult> GetAll(int? categoryId)
        {
            var sports = await _sportService.GetAllSportsAsync(categoryId);
            return Ok(sports);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateSport")]
        public async Task<IActionResult> Update(UpdateSportDto updateSportDto)
        {
            var sport = await _sportService.UpdateSportAsync(updateSportDto);
            return Ok(sport);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteSport")]
        public async Task<IActionResult> Delete(int id)
        {
            var sport = await _sportService.DeleteSportAsync(id);
            return Ok(sport);
        }
    }
}
