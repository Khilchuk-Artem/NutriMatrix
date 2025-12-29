using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Context;
using FoodCatalog.Persistance.Redis;
using FoodCatalog.Persistance.Specifications.Foods;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Features.Foods.Queries
{
    public class GetFoodByIdQuery : IRequest<FoodDTO>
    {
        public long Id { get; set; }
        public long[]? IncludeNutrientIds { get; set; }
    }

    public class GetFoodByIdQueryHandler : IRequestHandler<GetFoodByIdQuery, FoodDTO>
    {
        private readonly RedisCollection<FoodRedis> _foodCollection;
        private readonly RedisCollection<MeasureRedis> _measureCollection;
        private readonly IRepository<Food> _foodRepository;

        public GetFoodByIdQueryHandler(
            RedisCollection<FoodRedis> foodCollection,
            RedisCollection<MeasureRedis> measureCollection,
            IRepository<Food> foodRepository)
        {
            _foodCollection = foodCollection;
            _measureCollection = measureCollection;
            _foodRepository = foodRepository;
        }

        public async Task<FoodDTO> Handle(GetFoodByIdQuery request, CancellationToken cancellationToken)
        {
            var foodRedis = await _foodCollection
                .FirstOrDefaultAsync(f => f.Id == request.Id);
            if (foodRedis != null)
            {
                if (request.IncludeNutrientIds != null)
                {
                    foodRedis.FoodNutrients = foodRedis.FoodNutrients
                        .Where(fn => request.IncludeNutrientIds.Contains(fn.NutrientId))
                        .ToList();
                }

                var measures = await _measureCollection.Where(m => m.FoodId == request.Id).ToListAsync();

                return new FoodDTO
                {
                    Id = foodRedis.Id,
                    Name = foodRedis.Name,
                    Photo = foodRedis.Photo,
                    Barcode = foodRedis.Barcode,
                    FoodNutrients = foodRedis.FoodNutrients?
                        .Where(n => !n.IsDeleted)
                        .Select(n => new FoodNutrientIn100gDto { NutrientId = n.NutrientId, Amount = n.Amount })
                        .ToList(),
                    Measures = measures?
                        .Select(m => new MeasureDto { Id = m.Id, Name = m.Name, WeightInGrams = m.WeightInGrams })
                        .ToList()
                };
            }

            var spec = new FoodByIdSpecification(request.Id);
            var food = await _foodRepository.Get(request.Id, spec);
            if (food == null) return null;

            var foodRedisToCache = new FoodRedis
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients
                    .Select(n => new FoodNutrientIn100g { NutrientId = n.NutrientId, Amount = n.Amount, IsDeleted = n.IsDeleted })
                    .ToList()
            };
            await _foodCollection.InsertAsync(foodRedisToCache);

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
                FoodNutrients = food.FoodNutrients?
                    .Where(n => !n.IsDeleted)
                    .Select(n => new FoodNutrientIn100gDto { NutrientId = n.NutrientId, Amount = n.Amount })
                    .ToList(),
                Measures = food.Measures?
                    .Select(m => new MeasureDto { Id = m.Id, Name = m.Name, WeightInGrams = m.WeightInGrams })
                    .ToList()
            };
        }
    }
}
