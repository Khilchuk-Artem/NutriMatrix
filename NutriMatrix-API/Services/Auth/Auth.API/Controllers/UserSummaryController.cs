using Auth.Application.DTO;
using Auth.Application.Features.UserSummary.Commands;
using Auth.Application.Features.UserSummary.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSummaryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserSummaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserSummary(string userId)
        {
            var query = new GetUserSummaryQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound("User summary not found");

            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserSummary(UpdateUserDTO dto, string userId)
        {
            var command = new UpdateUserSummaryCommand { UpdateUserDTO = dto, UserId = userId };
            var result = await _mediator.Send(command);

            if (result == null)
                return BadRequest("Failed to update user summary");

            return Ok(result);
        }
    }
}