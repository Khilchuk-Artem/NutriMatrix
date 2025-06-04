using Microsoft.AspNetCore.Mvc;
using RecommendationService.Api.Models.Dto;
using RecommendationService.Api.Services.RecommendationService;

namespace RecommendationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController: ControllerBase
    {
        private readonly IRecipeRecommendationService _recipeRecommendationService;

        public RecommendationController(IRecipeRecommendationService recipeRecommendationService)
        {
            _recipeRecommendationService = recipeRecommendationService;
        }

        [HttpPost("get-recommendation")]
        public async Task<IActionResult> Recommendation(RecommendationRequestDto dto)
        {
            var data = await _recipeRecommendationService.GetRecommendationAsync(dto);

            return Ok(data);
        }
    }
}
