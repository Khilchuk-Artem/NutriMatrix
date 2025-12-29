using FoodRecords.Application.Features.FoodRecords.Commands;
using FoodRecords.Application.Features.FoodRecords.Queries;
using FoodRecords.Application.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoodRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddFoodRecordDto dto)
        {
            var command = new AddFoodRecordCommand { Dto = dto };
            var addedRecord = await _mediator.Send(command);
            if (addedRecord == null)
                return BadRequest("Failed to add record");
            return CreatedAtAction(nameof(Get), new { id = addedRecord.Id }, addedRecord);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var command = new DeleteFoodRecordCommand { Id = id };
            var deletedRecord = await _mediator.Send(command);
            if (deletedRecord == null)
                return NotFound();
            return Ok(deletedRecord);
        }

        [HttpGet("{id:long}", Name = "GetFoodRecordById")]
        public async Task<IActionResult> Get(long id)
        {
            var query = new GetFoodRecordQuery { Id = id };
            var recordDto = await _mediator.Send(query);
            if (recordDto == null)
                return NotFound();
            return Ok(recordDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string userId,
            [FromQuery] bool sortByDateAsc = true,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is required");

            var query = new GetAllFoodRecordsQuery
            {
                UserId = userId,
                SortByDateAsc = sortByDateAsc,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var records = await _mediator.Send(query);
            return Ok(records);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateFoodRecordDto dto)
        {
            var command = new UpdateFoodRecordCommand { Id = id, Dto = dto };
            var updatedRecord = await _mediator.Send(command);
            if (updatedRecord == null)
                return NotFound();
            return Ok(updatedRecord);
        }
    }
}
