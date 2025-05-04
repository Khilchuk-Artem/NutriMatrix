namespace RecommendationService.Api.Models.Dto
{
    public class RecommendationRequestDto
    {
        public IEnumerable<RecipeRequestDto> RecipeRequests { get; set; }
        public Dictionary<int, float> NutritionalGoals { get; set; }
    }
}
