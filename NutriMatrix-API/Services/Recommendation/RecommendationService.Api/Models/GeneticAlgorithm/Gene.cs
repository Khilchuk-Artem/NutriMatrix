using RecommendationService.Api.Models.Redis;

namespace RecommendationService.Api.Services.RecommendationService
{
    public partial class RecipeRecommendationService
    {
        private class Gene
        {
            public RecipeShortcutRedis Recipe;
            public float Amount;
        }
    }
}
