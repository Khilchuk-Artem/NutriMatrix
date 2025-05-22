using BuildingBlocks.Nutrionix.Refit;
using FoodCatalog.Api.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private INutrionixApi _nutrionixApi;
        private FoodCatalogDbContext _context;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, INutrionixApi nutrionixApi, FoodCatalogDbContext context)
        {
            _logger = logger;
            _nutrionixApi = nutrionixApi;
            _context = context;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            //var res = await _nutrionixApi.GetNutritionFromNaturalInput(new NutritionQueryRequest { Query = "rice" });
            var res = await _context.Foods.Include(f => f.Measures).Include(f => f.FoodNutrients).Take(1).ToListAsync();
            return Ok(res);
        }
    }
}
