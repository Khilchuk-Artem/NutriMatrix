namespace RecommendationService.Api.Models.Dto
{
    public class RecipeRequestDto
    {
        public IEnumerable<Guid>? IncludeIngredientIds { get; set; }
        public IEnumerable<Guid>? ExcludeIngredientIds { get; set; }

        public string? Category { get; set; }
    }
}
