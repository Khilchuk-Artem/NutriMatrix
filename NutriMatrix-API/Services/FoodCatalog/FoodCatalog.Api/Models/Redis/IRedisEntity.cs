using Redis.OM.Modeling;

namespace FoodCatalog.Api.Models.Redis
{
    public interface IRedisEntity
    {
        public long Id { get; set; }
    }
}
