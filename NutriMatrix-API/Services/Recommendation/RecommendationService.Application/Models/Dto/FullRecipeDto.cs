namespace RecommendationService.Application.Models.Dto
{
    public class FullRecipeDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public float Servings { get; set; }
        public string Description { get; set; }
        public string Directions { get; set; }
        public string PhotoUrl { get; set; }
        public List<IngredientMeasureDto> Ingredients { get; set; }
        public List<NutrientAmountDto> Nutrients { get; set; }
    }
}
