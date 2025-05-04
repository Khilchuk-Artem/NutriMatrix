using Redis.OM.Modeling;

namespace FoodCatalog.Api.Models.Redis
{
    [Document(StorageType = StorageType.Json)]
    public class MeasureRedis : IRedisEntity
    {
        [RedisIdField]
        public Guid Id { get; set; }

        [Indexed]
        public string Name { get; set; }
        public int WeightInGrams { get; set; }

        [Indexed]
        public Guid FoodId { get; set; }
    }

}
