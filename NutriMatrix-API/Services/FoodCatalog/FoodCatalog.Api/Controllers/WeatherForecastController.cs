using BuildingBlocks.Nutrionix.Refit;
using Microsoft.AspNetCore.Mvc;

namespace FoodCatalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private INutrionixApi _nutrionixApi;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, INutrionixApi nutrionixApi)
        {
            _logger = logger;
            _nutrionixApi = nutrionixApi;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            var res = await _nutrionixApi.GetNutritionFromNaturalInput(new NutritionQueryRequest { Query = "rice" });

            return Ok(res);
        }
    }
}
