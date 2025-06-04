using FoodCatalog.Api.Controllers.FoodCatalog.Api.Models.Dto;
using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Dto;
using FoodCatalog.Api.Models.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Modeling;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Controllers
{
    namespace FoodCatalog.Api.Models.Dto
    {
        public class MeasureWithFoodDto
        {
            public long Id { get; set; }
            public string Name { get; set; } = null!;
            public double WeightInGrams { get; set; }

            public FoodShortcutDTO Food { get; set; } = null!;
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class MeasureController : ControllerBase
    {
        private readonly RedisCollection<MeasureRedis> _measureCollection;
        private readonly RedisCollection<FoodRedis> _foodCollection;
        private readonly FoodCatalogDbContext _dbContext;
        public MeasureController(
            RedisCollection<MeasureRedis> measureCollection,
            RedisCollection<FoodRedis> foodCollection,
            FoodCatalogDbContext dbContext)
        {
            _measureCollection = measureCollection;
            _foodCollection = foodCollection;
            _dbContext = dbContext;
        }

        [HttpGet("{id:long}", Name = "GetMeasureById")]
        public async Task<IActionResult> Get(long id)
        {
            var measure = await _measureCollection.FirstOrDefaultAsync(m => m.Id == id);
            if (measure == null)
            {
                var measureDomain = await _dbContext.Measures.FirstOrDefaultAsync(m => m.Id == id);
                if (measureDomain == null) return NotFound();

                measure = new MeasureRedis()
                {
                    Id = measureDomain.Id,
                    Name = measureDomain.Name,
                    WeightInGrams = measureDomain.WeightInGrams,
                    FoodId = measureDomain.FoodId
                };
            }

            var food = await _foodCollection.FirstOrDefaultAsync(f => f.Id == measure.FoodId);
            if (food == null) return NotFound($"Food with ID {measure.FoodId} not found.");

            var dto = new MeasureWithFoodDto
            {
                Id = measure.Id,
                Name = measure.Name,
                WeightInGrams = measure.WeightInGrams,
                Food = new FoodShortcutDTO
                {
                    Id = food.Id,
                    Name = food.Name,
                    Nutrients = food.FoodNutrients?
                        .Where(n => !n.IsDeleted)
                        .Select(n => new FoodNutrientIn100gDto
                        {
                            NutrientId = n.NutrientId,
                            Amount = n.Amount
                        })
                        .ToList() ?? new List<FoodNutrientIn100gDto>()
                }
            };

            return Ok(dto);
        }
    }
}
