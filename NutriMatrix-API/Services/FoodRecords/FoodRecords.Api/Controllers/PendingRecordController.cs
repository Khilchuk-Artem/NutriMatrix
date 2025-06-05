using FoodRecords.Api.Data;
using FoodRecords.Api.Models.Domain;
using FoodRecords.Api.Services.MealFetcher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PendingRecordController:ControllerBase
    {
        private readonly FoodRecordsDbContext _dbContext;
        private readonly IMealFetcher _mealFetcher;
        public PendingRecordController(FoodRecordsDbContext dbContext, IMealFetcher mealFetcher)
        {
            _dbContext = dbContext;
            _mealFetcher = mealFetcher;
        }

        // GET: api/Planning
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PendingRecord>>> GetPendingAdditions()
        {
            var pendingAdditions = await _dbContext.PendingRecords
                .Where(pa => !pa.IsDeleted)
                .ToListAsync();
            return Ok(pendingAdditions);
        }

        // GET: api/Planning/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PendingRecord>> GetPendingAddition(long id)
        {
            var pendingAddition = await _dbContext.PendingRecords
                .FirstOrDefaultAsync(pa => pa.Id == id && !pa.IsDeleted);

            if (pendingAddition == null)
            {
                return NotFound();
            }

            return Ok(pendingAddition);
        }

        // POST: api/Planning
        [HttpPost]
        public async Task<ActionResult<PendingRecord>> CreatePendingAddition(PendingAdditionDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
            {
                return BadRequest("Invalid ConsumableType or model state");
            }

            var pendingAddition = new PendingRecord
            {
                ConsumableType = dto.ConsumableType,
                Amount = dto.Amount,
                UserId = dto.UserId,
                ConsumableId = dto.ConsumableId,
                DatePending = dto.DatePending,
                IsDeleted = false
            };

            _dbContext.PendingRecords.Add(pendingAddition);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPendingAddition), new { id = pendingAddition.Id }, pendingAddition);
        }

        // PUT: api/Planning/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePendingAddition(long id, PendingAdditionDto dto)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
            {
                return BadRequest("Invalid ConsumableType or model state");
            }

            var pendingAddition = await _dbContext.PendingRecords
                .FirstOrDefaultAsync(pa => pa.Id == id && !pa.IsDeleted);

            if (pendingAddition == null)
            {
                return NotFound();
            }

            pendingAddition.ConsumableType = dto.ConsumableType;
            pendingAddition.Amount = dto.Amount;
            pendingAddition.UserId = dto.UserId;
            pendingAddition.DatePending = dto.DatePending;
            pendingAddition.ConsumableId = dto.ConsumableId;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Planning/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePendingAddition(long id)
        {
            var pendingAddition = await _dbContext.PendingRecords
                .FirstOrDefaultAsync(pa => pa.Id == id && !pa.IsDeleted);

            if (pendingAddition == null)
            {
                return NotFound();
            }

            pendingAddition.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        // DELETE: api/Planning/5
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmAddition(long id)
        {
            var pendingAddition = await _dbContext.PendingRecords
                .FirstOrDefaultAsync(pa => pa.Id == id && !pa.IsDeleted);

            if (pendingAddition == null)
            {
                return NotFound();
            }

            pendingAddition.IsDeleted = true;

            if (pendingAddition.ConsumableType == ConsumableType.Food)
            {
                var foodRecord = new FoodRecord()
                {
                    DateEaten = pendingAddition.DatePending,
                    UserId = pendingAddition.UserId,
                    FoodMeasureId = pendingAddition.ConsumableId,
                    Amount = pendingAddition.Amount
                };

                _dbContext.FoodRecords.Add(foodRecord);
            }
            else
            {
                var dto = await _mealFetcher.FetchMealAsync(pendingAddition.ConsumableId);

                var mealRecord = new MealRecord
                {
                    MealId = dto.Id,
                    DateEaten = pendingAddition.DatePending,
                    UserId = pendingAddition.UserId,
                    ServingsEaten = pendingAddition.Amount,
                    IsDeleted = false,
                    IngredientSnapshots = dto.FoodMeals
                    .Select(fm => new MealIngredientSnapshot
                    {
                        FoodMeasureId = fm.MeasureId,
                        Amount = fm.Quantity*pendingAddition.Amount/dto.TotalServings
                    })
                    .ToList()
                };

                _dbContext.MealRecords.Add(mealRecord);
            }

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
    public class PendingAdditionDto
    {
        public ConsumableType ConsumableType { get; set; }
        public float Amount { get; set; }
        public string UserId { get; set; }
        public long ConsumableId { get; set; }
        public DateTime DatePending { get; set; }
    }
}
