using FoodRecords.Application.Features.FoodPlans.Commands;
using FoodRecords.Application.Features.FoodPlans.Queries;
using FoodRecords.Application.Services.TaskSchedulerService;
using FoodRecords.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodPlanController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoodPlanController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodPlan>>> GetFoodPlans(
            [FromQuery] string userId = null,
            [FromQuery] bool recurringFirst = true,
            [FromQuery] string searchByName = null)
        {
            var query = new GetFoodPlansQuery
            {
                UserId = userId,
                RecurringFirst = recurringFirst,
                SearchByName = searchByName
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodPlan>> GetFoodPlan(long id)
        {
            var query = new GetFoodPlanByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<FoodPlan>> CreateFoodPlan(ScheduleDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                return BadRequest("Invalid ConsumableType or model state");

            var command = new CreateFoodPlanCommand { Dto = dto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetFoodPlan), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFoodPlan(long id, ScheduleDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                return BadRequest("Invalid ConsumableType or model state");

            if (dto.Id != id)
                return BadRequest("ID mismatch");

            try
            {
                var command = new UpdateFoodPlanCommand { Id = id, Dto = dto };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound();
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodPlan(long id)
        {
            try
            {
                var command = new DeleteFoodPlanCommand { Id = id };
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
