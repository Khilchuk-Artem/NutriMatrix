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
                var foodRedisCollection = _redisProvider.RedisCollection<FoodRedis>();
                var measureRedisCollection = _redisProvider.RedisCollection<MeasureRedis>();

                foreach (var food in addedFoods)
                {
                    var foodRedis = await foodRedisCollection.FirstOrDefaultAsync(e => e.Id == food.Id)
                                      ?? new FoodRedis { Id = food.Id };

                    foodRedis.Name = food.Name ?? foodRedis.Name;
                    foodRedis.Photo = food.Photo ?? foodRedis.Photo;
                    foodRedis.FoodNutrients = food.FoodNutrients;

                    await foodRedisCollection.InsertAsync(foodRedis);

                    if (food.Measures != null)
                    {
                        foreach (var measure in food.Measures)
                        {
                            var measureRedis = await measureRedisCollection.FirstOrDefaultAsync(m => m.Id == measure.Id)
                                                 ?? new MeasureRedis { Id = measure.Id };

                            measureRedis.Name = measure.Name;
                            measureRedis.WeightInGrams = measure.WeightInGrams;
                            measureRedis.FoodId = food.Id;

                            await measureRedisCollection.InsertAsync(measureRedis);
                        }
                    }
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
