using Redis.OM.Modeling;

namespace FoodCatalog.Api.Models.Redis
{
    [Document(
        StorageType = StorageType.Json,
        Prefixes = new[] { "Measure" }
    )]
    public class MeasureRedis : IRedisEntity
    {
        [RedisIdField]
        [Indexed]
        public long Id { get; set; }

        [Indexed]
        public string Name { get; set; }
        public float WeightInGrams { get; set; }

        [Indexed]
        public long FoodId { get; set; }
    }

}
