namespace RecommendationService.Application.Models.Dto
{
    public class RecommendationResponseDto
    {
        public IEnumerable<RecipeWithAmountDto> RecipesAndAmounts { get; set; }
        public Dictionary<int,float> Nutrients { get; set; }
        public float TotalDistance { get; set; }
        public long TimeToRespondInMs { get; set; }
    }
}
