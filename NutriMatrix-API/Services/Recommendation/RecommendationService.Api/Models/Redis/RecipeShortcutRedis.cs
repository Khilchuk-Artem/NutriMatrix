using Redis.OM.Modeling;
using System.Collections;

namespace RecommendationService.Api.Models.Redis
{
    [Document(
    StorageType = StorageType.Json,               // store as RedisJSON  
    Prefixes = new[] { "RecipeShortcut" }    // key prefix  
)]
    public class RecipeShortcutRedis : IRedisEntity
    {
        [RedisIdField]      // ← marks this as the document’s key
        [Indexed]           // ← lets you filter by Id in LINQ
        public long Id { get; set; }

        [Indexed]           // ← now you can do .Where(x=>x.RecipeId==…)
        public long RecipeId { get; set; }

        public float Servings { get; set; }

        [Indexed]
        public string Category { get; set; }

        [Indexed(CascadeDepth = 1)]
        public IEnumerable<Guid> IngredientIds { get; set; }

        public IDictionary<int, float> NutrientAmounts { get; set; }
    }
}
