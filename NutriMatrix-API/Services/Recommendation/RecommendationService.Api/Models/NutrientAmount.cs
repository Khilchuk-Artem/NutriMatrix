using RecommendationService.Api.Models.Redis;

namespace RecommendationService.Api.Models
{
    public class NutrientAmount : IEntity
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public int NutrientId { get; set; }
        public float Amount { get; set; }

        public Recipe Recipe { get; set; }
        public bool IsDeleted { get; set; }
    }
}
