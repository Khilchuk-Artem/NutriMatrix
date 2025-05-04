using Redis.OM.Modeling;
using System.Text.Json;

namespace RecommendationService.Api.Models.Redis
{
    [Document(
    StorageType = StorageType.Json,               // store as RedisJSON  
    Prefixes = new[] { "CategoryAverageNutrients" }    // key prefix  
    )]
    public class CategoryAverageNutrientsRedis
    {
        [RedisIdField]
        [Indexed]
        public string Category { get; set; }

        [Indexed]  // Add this attribute to index the Category field
        public string AmountsJson { get; set; }  
        public Dictionary<int, float> Amounts
        {
            get
            {
                return string.IsNullOrEmpty(AmountsJson) ? new Dictionary<int, float>() :
                    JsonSerializer.Deserialize<Dictionary<int, float>>(AmountsJson);
            }
        }
    }
}
