using Auth.API.Models.DTO;
using Auth.API.Services.UserSummaryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSummaryController : Controller
    {
        private readonly IUserSummaryService _userSummaryService;

        public UserSummaryController(IUserSummaryService userSummaryService)
        {
            _userSummaryService = userSummaryService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserSummary(string userId)
        {

            var result = await _userSummaryService.GetUserSummary(userId);

            if (result == null)
                return NotFound("User summary not found");

            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserSummary(UpdateUserDTO dto,string userId)
        {
            /*var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User ID not found in token");*/

            var result = await _userSummaryService.UpdateUserSummaryById(dto, userId);

            if (result == null)
                return BadRequest("Failed to update user summary");

            return Ok(result);
        }
    }
}
