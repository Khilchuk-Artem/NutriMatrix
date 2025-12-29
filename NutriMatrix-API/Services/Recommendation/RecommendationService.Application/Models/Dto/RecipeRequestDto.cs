namespace RecommendationService.Application.Models.Dto
{
    public class RecipeRequestDto
    {
        public List<string>? IncludeIngredientIds { get; set; }
        public List<string>? ExcludeIngredientIds { get; set; }

        public string? Category { get; set; }
    }
}
