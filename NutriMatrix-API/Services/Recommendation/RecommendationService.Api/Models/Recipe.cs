
namespace RecommendationService.Api.Models
{
    public class Recipe : IEntity
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public float Servings { get; set; }
        public string Description { get; set; }
        public string Directions { get; set; }
        public string PhotoUrl { get; set; }

        public IEnumerable<RecipeMeasure> Measures { get; set; }
        public bool IsDeleted { get; set; }
    }
}
