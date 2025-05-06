using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecommendationService.Api.Data;
using RecommendationService.Api.Models.Dto;
using RecommendationService.Api.Models.Redis;
using RecommendationService.Api.Services.RecommendationService;
using Redis.OM.Searching;
using System.Runtime.InteropServices;
using Redis.OM;
using Redis.OM.Contracts;
using RecommendationService.Api.Services.Qdrant;

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
        public WeatherForecastController(ILogger<WeatherForecastController> logger, RedisCollection<RecipeShortcutRedis> collection, IRecipeRecommendationService recipeRecommendationService, IQdrantService qdrantService)
        {
            _logger = logger;
            _collection = collection;
            _recipeRecommendationService = recipeRecommendationService;
            _qdrantService = qdrantService;
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
            var data = Seeding.GetRecipeShortcutRedis();
            await _qdrantService.CreateCollectionAsync();

            foreach (var a in data)
            {
                var key = await _collection.InsertAsync(a);
                await _qdrantService.InsertRecipeVectorAsync((int)a.Id,a.NutrientAmounts.Values,a.Category,new List<string>());
            }

            var res = await _collection.Take(20).ToListAsync();

            return Ok(res);
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
