using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Redis;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Contracts;

namespace FoodCatalog.Api.Data.Interceptors
{
    public class FoodRedisSyncInterceptor : SaveChangesInterceptor
    {
        private readonly IRedisConnectionProvider _redisProvider;

        public FoodRedisSyncInterceptor(RedisConnectionProvider redisProvider)
        {
            _redisProvider = redisProvider;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var addedFoods = context.ChangeTracker.Entries<Food>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            if (addedFoods.Any())
            {
                var redisCollection = _redisProvider.RedisCollection<FoodRedis>();

                foreach (var food in addedFoods)
                {
                    var existingFoodRedis = await redisCollection.FirstOrDefaultAsync(e => e.Id == food.Id);

                    var foodRedis = existingFoodRedis ?? new FoodRedis { Id = food.Id };

                    foodRedis.Name = food.Name ?? foodRedis.Name;
                    foodRedis.Photo = food.Photo ?? foodRedis.Photo;

                    if (food.Measures != null && food.Measures.Any())
                    {
                        foodRedis.Measures = food.Measures;
                    }

                    if (food.FoodNutrients != null && food.FoodNutrients.Any())
                    {
                        foodRedis.FoodNutrients = food.FoodNutrients;
                    }

                    await redisCollection.InsertAsync(foodRedis);
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
