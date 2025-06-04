namespace RecommendationService.Api.Models.Dto
{
    public class RecommendationRequestDto
    {
        public IList<RecipeRequestDto> RecipeRequests { get; set; }
        public Dictionary<int, float> NutritionalGoals { get; set; }
    }
}
