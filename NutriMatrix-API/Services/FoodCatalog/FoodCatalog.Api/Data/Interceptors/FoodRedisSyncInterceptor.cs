using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Redis;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Data.Interceptors
{
    public class FoodRedisSyncInterceptor : SaveChangesInterceptor
    {
        private readonly RedisCollection<FoodRedis> _foodRedisCollection;
        private readonly RedisCollection<MeasureRedis> _measureRedisCollection;

        public FoodRedisSyncInterceptor(RedisCollection<FoodRedis> foodRedisCollection, RedisCollection<MeasureRedis> measureRedisCollection)
        {
            _foodRedisCollection = foodRedisCollection;
            _measureRedisCollection = measureRedisCollection;
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

                foreach (var food in addedFoods)
                {
                    await context.Entry(food).Collection(f => f.FoodNutrients).LoadAsync(cancellationToken);
                    await context.Entry(food).Collection(f => f.Measures).LoadAsync(cancellationToken);

                    var foodRedis = await _foodRedisCollection.FirstOrDefaultAsync(e => e.Id == food.Id)
                                      ?? new FoodRedis { Id = food.Id };

                    foodRedis.Name = food.Name ?? foodRedis.Name;
                    foodRedis.Photo = food.Photo ?? foodRedis.Photo;
                    foodRedis.Barcode = food.Barcode;
                    foodRedis.FoodNutrients = food.FoodNutrients?
                        .Select(n => new FoodNutrientIn100g
                        {
                            Id = n.Id,
                            FoodId = n.FoodId,
                            NutrientId = n.NutrientId,
                            Amount = n.Amount,
                            IsDeleted = n.IsDeleted
                        }).ToList();

                    await _foodRedisCollection.InsertAsync(foodRedis);

                    if (food.Measures != null)
                    {
                        foreach (var measure in food.Measures)
                        {
                            var measureRedis = await _measureRedisCollection.FirstOrDefaultAsync(m => m.Id == measure.Id)
                                                 ?? new MeasureRedis { Id = measure.Id };

                            measureRedis.Name = measure.Name;
                            measureRedis.WeightInGrams = measure.WeightInGrams;
                            measureRedis.FoodId = food.Id;

                            await _measureRedisCollection.InsertAsync(measureRedis);
                        }
                    }
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}