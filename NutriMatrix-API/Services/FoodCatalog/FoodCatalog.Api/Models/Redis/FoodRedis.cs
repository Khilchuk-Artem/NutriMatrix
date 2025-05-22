using FoodCatalog.Api.Models.Domain;
using Redis.OM.Modeling;

namespace FoodCatalog.Api.Models.Redis
{
    [Document(
        StorageType = StorageType.Json,
        Prefixes = new[] { "Food" }
    )]
    public class FoodRedis : IRedisEntity
    {
        [RedisIdField]
        [Indexed]
        public long Id { get; set; }
        [Indexed]
        [Searchable]
        public string Name { get; set; }
        public string Photo { get; set; }
        [Indexed]
        public string? Barcode { get; set; }

        public IEnumerable<FoodNutrientIn100g> FoodNutrients { get; set; }
    }
}
