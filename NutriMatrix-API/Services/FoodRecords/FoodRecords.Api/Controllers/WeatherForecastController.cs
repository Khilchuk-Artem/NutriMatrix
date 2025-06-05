using BuildingBlocks.Messaging.Responses;
using FoodRecords.Api.Services.MealFetcher;
using Microsoft.AspNetCore.Mvc;

namespace FoodRecords.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMealFetcher _fetcher;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMealFetcher fetcher)
        {
            _logger = logger;
            _fetcher = fetcher;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<MealResponseDto> GetAsync()
        {
            return await _fetcher.FetchMealAsync(1);
        }
    }
}
