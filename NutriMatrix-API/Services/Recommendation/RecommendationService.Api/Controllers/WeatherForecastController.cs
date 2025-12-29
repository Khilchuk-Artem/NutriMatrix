using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecommendationService.Api.Services.RecommendationService;
using Redis.OM.Searching;
using System.Runtime.InteropServices;
using Redis.OM;
using Redis.OM.Contracts;
using RecommendationService.Persistance.Redis.Entities;
using RecommendationService.Persistance.Qdrant;
using RecommendationService.Persistance.Context;
using RecommendationService.Application.Models.Dto;

namespace RecommendationService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly RedisCollection<RecipeShortcutRedis> _collection;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRecipeRecommendationService _recipeRecommendationService;
        private readonly IQdrantService _qdrantService;
        private readonly RecipeDbContext _dbContext;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, RedisCollection<RecipeShortcutRedis> collection, IRecipeRecommendationService recipeRecommendationService, IQdrantService qdrantService, RecipeDbContext dbContext)
        {
            _logger = logger;
            _collection = collection;
            _recipeRecommendationService = recipeRecommendationService;
            _qdrantService = qdrantService;
            _dbContext = dbContext;
        }

        [HttpGet("GetWeatherForecast")]
        public async Task<IActionResult> GetAsync(int id)
        {
            //var res = await _collection.ToListAsync();

            return Ok(_collection.Where(r=>r.Id==id).ToList());
        }
        [HttpGet("Seed")]
        public async Task<IActionResult> SeedAsync()
        {
            await _qdrantService.CreateCollectionAsync();

            await foreach (var record in _dbContext.Recipes.Include(r=>r.Measures).AsAsyncEnumerable())
            {
                var redis = await _collection.FirstOrDefaultAsync(r => r.Id == record.Id);

                if (redis.NutrientAmounts.Count == 161)
                {
                    var ingredients = record.Measures.Select(m => m.FoodId.ToString()).ToList();
                    var nutrients = redis.NutrientAmounts.Select(kv => kv.Value).ToList();
                    await _qdrantService.InsertRecipeVectorAsync((int)record.Id, nutrients, record.Category, ingredients);
                }
                if (redis.NutrientAmounts.Count != 161)
                {
                    var hmmm = 1;
                }

            }

            return Ok();
        }
        [HttpPost("Recommendation")]
        public async Task<IActionResult> Recommendation(RecommendationRequestDto dto)
        {
            var data = await _recipeRecommendationService.GetRecommendationAsync(dto);

            return Ok(data);
        }
        [HttpPost("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var entities = await _collection.ToListAsync();
            while (entities.Any())
            {
                foreach (var a in entities)
                {
                    await _collection.DeleteAsync(a);
                }
                entities = await _collection.ToListAsync();
            }


            return Ok();
        }
        [HttpPost("GetNearestNeighbors")]
        public async Task<IActionResult> GetNearestNeighbors()
        {
            var vector = Enumerable.Repeat(0f, 161).ToArray();
            vector[0] = 50;
            vector[1] = 25;
            var res = await _qdrantService.FindKNearestNeighborsAsync(5, vector);


            return Ok(res);
        }
    }
}
