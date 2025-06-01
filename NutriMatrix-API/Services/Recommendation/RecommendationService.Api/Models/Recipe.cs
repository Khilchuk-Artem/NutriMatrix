
namespace RecommendationService.Api.Models
{
    public class Recipe : IEntity
    {
        public long Id { get; set; }
        public string Category { get; set; }
        public float? Servings { get; set; }
        public string Description { get; set; }
        public string Directions { get; set; }
        public string PhotoUrl { get; set; }
        public string Title { get; set; }
        public ICollection<RecipeMeasure> Measures { get; set; }
        public ICollection<NutrientAmount> NutrientsPerTotalServings { get; set; }
        public bool IsDeleted { get; set; }
    }
}
