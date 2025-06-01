using FoodRecords.Api.Data;
using FoodRecords.Api.Models.Domain;
using FoodRecords.Api.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealRecordController : ControllerBase
    {
        private readonly FoodRecordsDbContext _dbContext;

        public MealRecordController(FoodRecordsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddMealRecordDto dto)
        {
            var mealRecord = new MealRecord
            {
                MealId = dto.MealId,
                DateEaten = dto.DateEaten,
                UserId = dto.UserId,
                ServingsEaten = dto.ServingsEaten,
                IsDeleted = false,
                IngredientSnapshots = dto.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshot
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            };

            _dbContext.MealRecords.Add(mealRecord);
            await _dbContext.SaveChangesAsync();

            var mealRecordDto = new MealRecordDto
            {
                MealId = mealRecord.MealId,
                Id = mealRecord.Id,
                DateEaten = mealRecord.DateEaten,
                UserId = mealRecord.UserId,
                ServingsEaten = mealRecord.ServingsEaten,
                IngredientSnapshots = mealRecord.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshotDto
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            };

            return CreatedAtAction(nameof(Get), new { id = mealRecord.Id }, mealRecordDto);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            
            var mealRecord = await _dbContext
                .MealRecords
                .Include(mr=>mr.IngredientSnapshots)
                .FirstOrDefaultAsync(mr => mr.Id == id && !mr.IsDeleted);

            if (mealRecord == null)
            {
                return NotFound();
            }

            mealRecord.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            var mealRecordDto = new MealRecordDto
            {
                Id = mealRecord.Id,
                DateEaten = mealRecord.DateEaten,
                UserId = mealRecord.UserId,
                ServingsEaten = mealRecord.ServingsEaten,
                IngredientSnapshots = mealRecord.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshotDto
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            };

            return Ok(mealRecordDto);
        }

        [HttpGet("{id:long}", Name = "GetMealRecordById")]
        public async Task<IActionResult> Get(long id)
        {
            var mealRecord = await _dbContext.MealRecords
                .Where(mr => mr.Id == id && !mr.IsDeleted)
                .Select(mr => new MealRecordDto
                {
                    MealId = mr.MealId,
                    Id = mr.Id,
                    DateEaten = mr.DateEaten,
                    UserId = mr.UserId,
                    ServingsEaten = mr.ServingsEaten,
                    IngredientSnapshots = mr.IngredientSnapshots
                        .Select(s => new MealIngredientSnapshotDto
                        {
                            FoodMeasureId = s.FoodMeasureId,
                            Amount = s.Amount,
                            Id = s.Id
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (mealRecord == null)
            {
                return NotFound();
            }

            return Ok(mealRecord);
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string userId,
            [FromQuery] bool sortByDateAsc = true,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null
            )
        {
            var query = _dbContext.MealRecords
                .Where(mr => mr.UserId == userId && !mr.IsDeleted)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                query = query.Where(mr => mr.DateEaten >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(mr => mr.DateEaten <= dateTo.Value);
            }

            query = sortByDateAsc
                ? query.OrderBy(mr => mr.DateEaten)
                : query.OrderByDescending(mr => mr.DateEaten);

            var records = query
                .Select(mr => new MealRecordDto
                {
                    MealId = mr.MealId,
                    Id = mr.Id,
                    DateEaten = mr.DateEaten,
                    UserId = mr.UserId,
                    ServingsEaten = mr.ServingsEaten,
                    IngredientSnapshots = mr.IngredientSnapshots
                        .Select(s => new MealIngredientSnapshotDto
                        {
                            FoodMeasureId = s.FoodMeasureId,
                            Amount = s.Amount,
                           
                        })
                        .ToList()
                })
                .ToList();

            return Ok(records);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateMealRecordDto dto)
        {
            var mealRecord = await _dbContext.MealRecords
                .Include(mr => mr.IngredientSnapshots)
                .FirstOrDefaultAsync(mr => mr.Id == id && !mr.IsDeleted);

            if (mealRecord == null)
            {
                return NotFound();
            }

            mealRecord.DateEaten = dto.DateEaten;
            mealRecord.ServingsEaten = dto.ServingsEaten;
            mealRecord.MealId = dto.MealId;

            _dbContext.RemoveRange(mealRecord.IngredientSnapshots);

            mealRecord.IngredientSnapshots = dto.IngredientSnapshots
                .Select(s => new MealIngredientSnapshot
                {
                    FoodMeasureId = s.FoodMeasureId,
                    Amount = s.Amount,
                    IsDeleted = false
                })
                .ToList();

            await _dbContext.SaveChangesAsync();

            var mealRecordDto = new MealRecordDto
            {
                MealId = mealRecord.MealId,
                Id = mealRecord.Id,
                DateEaten = mealRecord.DateEaten,
                UserId = mealRecord.UserId,
                ServingsEaten = mealRecord.ServingsEaten,
                IngredientSnapshots = mealRecord.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshotDto
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            };

            return Ok(mealRecordDto);
        }

    }
}
