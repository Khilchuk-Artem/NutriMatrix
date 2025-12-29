using Redis.OM.Modeling;

namespace FoodCatalog.Persistance.Redis
{
    public interface IRedisEntity
    {
        public long Id { get; set; }
    }
}
