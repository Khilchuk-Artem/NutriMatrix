using FoodRecords.Application.Dto;
using FoodRecords.Application.Features.PendingRecords.Commands;
using FoodRecords.Application.Features.PendingRecords.Queries;
using FoodRecords.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PendingRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PendingRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PendingRecord>>> GetPendingAdditions(
            [FromQuery] string userId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = new GetPendingRecordsQuery { UserId = userId, StartDate = startDate, EndDate = endDate };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PendingRecord>> GetPendingAddition(long id)
        {
            var query = new GetPendingRecordByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PendingRecord>> CreatePendingAddition(PendingAdditionDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                return BadRequest("Invalid ConsumableType or model state");

            var command = new CreatePendingRecordCommand { Dto = dto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetPendingAddition), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePendingAddition(long id, PendingAdditionDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                return BadRequest("Invalid ConsumableType or model state");

            try
            {
                var command = new UpdatePendingRecordCommand { Id = id, Dto = dto };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePendingAddition(long id)
        {
            try
            {
                var command = new DeletePendingRecordCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmAddition(long id)
        {
            try
            {
                var command = new ConfirmPendingRecordCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
        }
    }
}
