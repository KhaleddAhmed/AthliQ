using AthliQ.Core.DTOs.Sport;
using AthliQ.Core.Service.Contract;
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
        public async Task<IActionResult> Create([FromBody]CreateSportDto createSportDto)
        {
            var sport = await _sportService.CreateSportAsync(createSportDto);
            return Ok(sport);
        }
    }
}
