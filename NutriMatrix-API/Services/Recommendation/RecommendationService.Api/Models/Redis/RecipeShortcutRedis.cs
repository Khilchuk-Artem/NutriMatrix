using Redis.OM;
using Redis.OM.Modeling;
using Redis.OM.Modeling.Vectors;
using System.Collections;

namespace RecommendationService.Api.Models.Redis
{
    [Document(
    StorageType = StorageType.Json,
    Prefixes = new[] { "RecipeShortcut" }
)]
    public class RecipeShortcutRedis : IRedisEntity
    {
        [RedisIdField]
        [Indexed]
        public long Id { get; set; }

        [Indexed]
        public long RecipeId { get; set; }

        public float Servings { get; set; }

        [Indexed]
        public string Category { get; set; }

        [Indexed(CascadeDepth = 1)]
        public IEnumerable<Guid> IngredientIds { get; set; }
        public IDictionary<int, float> NutrientAmounts { get; set; }
    }
}
