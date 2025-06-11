using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Dto;
using FoodCatalog.Api.Models.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Features.Queries
{
    public class GetFoodByBarcodeQuery : IRequest<FoodDTO>
    {
        public string Barcode { get; set; }
        public long[]? IncludeNutrientIds { get; set; }
    }

    public class GetFoodByBarcodeQueryHandler : IRequestHandler<GetFoodByBarcodeQuery, FoodDTO>
    {
        private readonly RedisCollection<FoodRedis> _foodCollection;
        private readonly RedisCollection<MeasureRedis> _measureCollection;
        private readonly FoodCatalogDbContext _dbContext;

        public GetFoodByBarcodeQueryHandler(RedisCollection<FoodRedis> foodCollection, RedisCollection<MeasureRedis> measureCollection, FoodCatalogDbContext dbContext)
        {
            _foodCollection = foodCollection;
            _measureCollection = measureCollection;
            _dbContext = dbContext;
        }

        public async Task<FoodDTO> Handle(GetFoodByBarcodeQuery request, CancellationToken cancellationToken)
        {
            var cachedFood = await _foodCollection.FirstOrDefaultAsync(f => f.Barcode == request.Barcode);
            if (cachedFood != null)
            {
                var m1 = await _measureCollection.Where(m => m.FoodId == cachedFood.Id).ToListAsync();
                var m2 = new List<Measure>();
                if (m1.Count == 0) m2 = await _dbContext.Measures.Where(m => m.FoodId == cachedFood.Id).ToListAsync();

                if (request.IncludeNutrientIds != null)
                {
                    cachedFood.FoodNutrients = cachedFood.FoodNutrients
                        .Where(fn => request.IncludeNutrientIds.Contains(fn.NutrientId))
                        .ToList();
                }

                var res = new FoodDTO
                {
                    Id = cachedFood.Id,
                    Name = cachedFood.Name,
                    Photo = cachedFood.Photo,
                    Barcode = cachedFood.Barcode,
                    FoodNutrients = cachedFood.FoodNutrients
                        .Where(n => !n.IsDeleted)
                        .Select(n => new FoodNutrientIn100gDto
                        {
                            NutrientId = n.NutrientId,
                            Amount = n.Amount
                        })
                        .ToList(),
                    Measures = m1.Count != 0
                        ? m1.Select(m => new MeasureDto
                        {
                            Id = m.Id,
                            Name = m.Name,
                            WeightInGrams = m.WeightInGrams
                        }).ToList()
                        : m2.Select(m => new MeasureDto
                        {
                            Id = m.Id,
                            Name = m.Name,
                            WeightInGrams = m.WeightInGrams
                        }).ToList()
                };

                return res;
            }

            var food = await _dbContext.Foods
                .Include(f => f.FoodNutrients)
                .Include(f => f.Measures)
                .FirstOrDefaultAsync(f => f.Barcode == request.Barcode);

            if (food == null) return null;

            if (food.FoodNutrients == null)
                food.FoodNutrients = new List<FoodNutrientIn100g>();
            else if (request.IncludeNutrientIds != null)
                food.FoodNutrients = food.FoodNutrients
                    .Where(fn => request.IncludeNutrientIds.Contains(fn.NutrientId))
                    .ToList();

            var foodRedis = new FoodRedis
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients.Select(n => new FoodNutrientIn100g
                {
                    NutrientId = n.NutrientId,
                    Amount = n.Amount,
                    IsDeleted = n.IsDeleted
                }).ToList()
            };
            await _foodCollection.InsertAsync(foodRedis);

            foreach (var measure in food.Measures)
            {
                var measureRedis = new MeasureRedis
                {
                    Id = measure.Id,
                    Name = measure.Name,
                    WeightInGrams = measure.WeightInGrams,
                    FoodId = food.Id
                };
                await _measureCollection.InsertAsync(measureRedis);
            }

            var resFromDb = new FoodDTO
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients
                    .Where(n => !n.IsDeleted)
                    .Select(n => new FoodNutrientIn100gDto
                    {
                        NutrientId = n.NutrientId,
                        Amount = n.Amount
                    })
                    .ToList(),
                Measures = food.Measures
                    .Select(m => new MeasureDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        WeightInGrams = m.WeightInGrams
                    })
                    .ToList()
            };

            return resFromDb;
        }
    }
}
