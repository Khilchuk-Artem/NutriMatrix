using FoodRecords.Api.Data;
using FoodRecords.Api.Models.Domain;
using FoodRecords.Api.Services.MealFetcher;
using FoodRecords.Api.Services.TaskSchedulerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodPlanController : ControllerBase
    {
        private readonly ITaskSchedulerService _taskSchedulerService;
        private readonly FoodRecordsDbContext _dbContext;
        private readonly IMealFetcher _mealFetcher;

        public FoodPlanController(ITaskSchedulerService taskSchedulerService, FoodRecordsDbContext dbContext, IMealFetcher mealFetcher)
        {
            _taskSchedulerService = taskSchedulerService;
            _dbContext = dbContext;
            _mealFetcher = mealFetcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodPlan>>> GetFoodPlans(
            [FromQuery] string userId = null,
            [FromQuery] bool recurringFirst = true,
            [FromQuery] string? searchByName = null)
        {
            var query = _dbContext.FoodPlans
                .Where(fp => !fp.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchByName))
            {
                query = query.Where(fp => fp.Name.Contains(searchByName));
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(fp => fp.UserId== userId);
            }
            if (recurringFirst)
            {
                query = query.OrderByDescending(fp => fp.IsRecurring);

            }
            
            var foodPlans = await query.ToListAsync();
            return Ok(foodPlans);
        }


        // GET: api/FoodPlan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodPlan>> GetFoodPlan(long id)
        {
            var foodPlan = await _dbContext.FoodPlans
                .FirstOrDefaultAsync(fp => fp.Id == id && !fp.IsDeleted);

            if (foodPlan == null)
            {
                return NotFound();
            }

            return Ok(foodPlan);
        }

        // POST: api/FoodPlan
        [HttpPost]
        public async Task<ActionResult<FoodPlan>> CreateFoodPlan(ScheduleDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
            {
                return BadRequest("Invalid ConsumableType or model state");
            }

            var foodPlan = await _taskSchedulerService.CreateScheduleAsync(dto);
            return CreatedAtAction(nameof(GetFoodPlan), new { id = foodPlan.Id }, foodPlan);
        }

        // PUT: api/FoodPlan/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFoodPlan(long id, ScheduleDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
            {
                return BadRequest("Invalid ConsumableType or model state");
            }

            if (dto.Id != id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var foodPlan = await _taskSchedulerService.UpdateScheduleAsync(dto);
                return Ok(foodPlan);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodPlan(long id, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId cannot be null or empty");

            try
            {
                await _taskSchedulerService.DeleteScheduleAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
        }
    }
}
