using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Data.Repositories.Repository;
using FoodCatalog.Api.Data.Specifications.Foods;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Dto;
using FoodCatalog.Api.Models.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Features.Foodss.Queries
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
        private readonly IRepository<Food> _foodRepository;

        public GetFoodByBarcodeQueryHandler(
            RedisCollection<FoodRedis> foodCollection,
            RedisCollection<MeasureRedis> measureCollection,
            IRepository<Food> foodRepository)
        {
            _foodCollection = foodCollection;
            _measureCollection = measureCollection;
            _foodRepository = foodRepository;
        }

        public async Task<FoodDTO> Handle(GetFoodByBarcodeQuery request, CancellationToken cancellationToken)
        {
            var cachedFood = await _foodCollection.FirstOrDefaultAsync(f => f.Barcode == request.Barcode);
            if (cachedFood != null)
            {
                var measures = await _measureCollection.Where(m => m.FoodId == cachedFood.Id).ToListAsync();

                if (request.IncludeNutrientIds != null)
                {
                    cachedFood.FoodNutrients = cachedFood.FoodNutrients
                        .Where(fn => request.IncludeNutrientIds.Contains(fn.NutrientId))
                        .ToList();
                }

                return new FoodDTO
                {
                    Id = cachedFood.Id,
                    Name = cachedFood.Name,
                    Photo = cachedFood.Photo,
                    Barcode = cachedFood.Barcode,
                    FoodNutrients = cachedFood.FoodNutrients
                        .Where(n => !n.IsDeleted)
                        .Select(n => new FoodNutrientIn100gDto { NutrientId = n.NutrientId, Amount = n.Amount })
                        .ToList(),
                    Measures = measures
                        .Select(m => new MeasureDto { Id = m.Id, Name = m.Name, WeightInGrams = m.WeightInGrams })
                        .ToList()
                };
            }

            var spec = new FoodByBarcodeSpecification(request.Barcode);
            var food = await _foodRepository.Get(0, spec); // ID is not used here, so pass 0
            if (food == null) return null;

            var foodRedis = new FoodRedis
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients
                    .Select(n => new FoodNutrientIn100g { NutrientId = n.NutrientId, Amount = n.Amount, IsDeleted = n.IsDeleted })
                    .ToList()
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

            if (request.IncludeNutrientIds != null)
            {
                food.FoodNutrients = food.FoodNutrients
                    .Where(fn => request.IncludeNutrientIds.Contains(fn.NutrientId))
                    .ToList();
            }

            return new FoodDTO
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients
                    .Where(n => !n.IsDeleted)
                    .Select(n => new FoodNutrientIn100gDto { NutrientId = n.NutrientId, Amount = n.Amount })
                    .ToList(),
                Measures = food.Measures
                    .Select(m => new MeasureDto { Id = m.Id, Name = m.Name, WeightInGrams = m.WeightInGrams })
                    .ToList()
            };
        }
    }
}
