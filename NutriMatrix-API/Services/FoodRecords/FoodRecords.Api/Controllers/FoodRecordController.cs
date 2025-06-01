using FoodRecords.Api.Models.Dto;
using FoodRecords.Api.Services.FoodRecords;
using Microsoft.AspNetCore.Mvc;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodRecordController : ControllerBase
    {
        private readonly IFoodRecordService _foodRecordService;

        public FoodRecordController(IFoodRecordService foodRecordService)
        {
            _foodRecordService = foodRecordService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddFoodRecordDto dto)
        {
            var addedRecord = await _foodRecordService.AddAsync(dto);
            if (addedRecord == null)
                return BadRequest("Failed to add record");

            return CreatedAtAction(nameof(Get), new { id = addedRecord.Id }, addedRecord);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deletedRecord = await _foodRecordService.DeleteAsync(id);
            if (deletedRecord == null)
                return NotFound();

            return Ok(deletedRecord);
        }

        [HttpGet("{id:long}", Name = "GetFoodRecordById")]
        public async Task<IActionResult> Get(long id)
        {
            var recordDto = await _foodRecordService.GetAsync(id);
            if (recordDto == null)
                return NotFound();

            return Ok(recordDto);
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string userId,
            [FromQuery] bool sortByDateAsc = true,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            if (userId == string.Empty)
                return BadRequest("UserId is required");

            var records = _foodRecordService.GetAll(userId, sortByDateAsc, dateFrom, dateTo);

            return Ok(records);
        }


        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateFoodRecordDto dto)
        {
            var updatedRecord = await _foodRecordService.UpdateAsync(id, dto);
            if (updatedRecord == null)
                return NotFound();

            return Ok(updatedRecord);
        }
    }
}
