using RecommendationService.Persistance.Redis.Entities;

namespace RecommendationService.Application.Models.Genetic
{
    public class Gene
    {
        public RecipeShortcutRedis Recipe;
        public float Amount;
    }
}
