using RecommendationService.Api.Models.Redis;

namespace RecommendationService.Api.Models
{
    public class NutrientAmount : IEntity
    {
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public int NutrientId { get; set; }
        public float Amount { get; set; }

        public Recipe Recipe { get; set; }
        public bool IsDeleted { get; set; }
    }
}
