using FoodRecords.Application.Features.MealRecords.Commands;
using FoodRecords.Application.Features.MealRecords.Queries;
using FoodRecords.Application.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MealRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddMealRecordDto dto)
        {
            var command = new AddMealRecordCommand { Dto = dto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var command = new DeleteMealRecordCommand { Id = id };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{id:long}", Name = "GetMealRecordById")]
        public async Task<IActionResult> Get(long id)
        {
            var query = new GetMealRecordByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string userId,
            [FromQuery] bool sortByDateAsc = true,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var query = new GetAllMealRecordsQuery
            {
                UserId = userId,
                SortByDateAsc = sortByDateAsc,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateMealRecordDto dto)
        {
            var command = new UpdateMealRecordCommand { Id = id, Dto = dto };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
