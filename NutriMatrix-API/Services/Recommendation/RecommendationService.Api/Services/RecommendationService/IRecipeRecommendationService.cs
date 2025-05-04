using RecommendationService.Api.Models.Dto;

namespace RecommendationService.Api.Services.RecommendationService
{
    public interface IRecipeRecommendationService
    {
        public Task<RecommendationResponseDto> GetRecommendationAsync(RecommendationRequestDto dto);
    }
}
