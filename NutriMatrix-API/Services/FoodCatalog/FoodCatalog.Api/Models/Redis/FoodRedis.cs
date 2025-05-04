using FoodCatalog.Api.Models.Domain;
using Redis.OM.Modeling;

namespace FoodCatalog.Api.Models.Redis
{
    [Document(StorageType = StorageType.Json)]
    public class FoodRedis : IRedisEntity
    {
        [RedisIdField]
        public Guid Id { get; set; }
        [Indexed]
        public string Name { get; set; }
        public string Photo { get; set; }

        public IEnumerable<FoodNutrientIn100g> FoodNutrients { get; set; }
    }
}
