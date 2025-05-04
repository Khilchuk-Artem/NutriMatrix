using Microsoft.AspNetCore.Mvc;
using RecommendationService.Api.Data;
using RecommendationService.Api.Models.Dto;
using RecommendationService.Api.Models.Redis;
using RecommendationService.Api.Services.RecommendationService;
using Redis.OM.Searching;

namespace RecommendationService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly RedisCollection<RecipeShortcutRedis> _collection;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRecipeRecommendationService _recipeRecommendationService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, RedisCollection<RecipeShortcutRedis> collection, IRecipeRecommendationService recipeRecommendationService)
        {
            _logger = logger;
            _collection = collection;
            _recipeRecommendationService = recipeRecommendationService;
        }

        [HttpGet("GetWeatherForecast")]
        public IActionResult Get()
        {
            return Ok(_collection.Where(r=>r.Category=="Pie").OrderBy(r=>r.Id).Take(20));
        }
        [HttpGet("Seed")]
        public async Task<IActionResult> SeedAsync()
        {
            var data = Seeding.GetRecipeShortcutRedis();

            foreach(var a in data)
            {
                await _collection.InsertAsync(a);
            }

            return Ok(_collection.OrderBy(r => r.Id).Take(20));
        }
        [HttpPost("Recommendation")]
        public async Task<IActionResult> Recommendation(RecommendationRequestDto dto)
        {
            var data = await _recipeRecommendationService.GetRecommendationAsync(dto);

            return Ok(data);
        }
    }
}
