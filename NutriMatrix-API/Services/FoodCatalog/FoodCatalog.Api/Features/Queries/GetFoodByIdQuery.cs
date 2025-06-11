using FoodCatalog.Api.Models.Dto;
using FoodCatalog.Api.Models.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Features.Queries
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

        public GetFoodByIdQueryHandler(RedisCollection<FoodRedis> foodCollection, RedisCollection<MeasureRedis> measureCollection)
        {
            _foodCollection = foodCollection;
            _measureCollection = measureCollection;
        }

        public async Task<FoodDTO> Handle(GetFoodByIdQuery request, CancellationToken cancellationToken)
        {
            var food = await _foodCollection.FirstOrDefaultAsync(f => f.Id == request.Id);
            if (food == null) return null;

            if (request.IncludeNutrientIds != null)
            {
                food.FoodNutrients = food.FoodNutrients.Where(fn => request.IncludeNutrientIds.Contains(fn.NutrientId)).ToList();
            }

            var measures = await _measureCollection.Where(m => m.FoodId == request.Id).ToListAsync();

            var res = new FoodDTO
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients?
                    .Where(n => !n.IsDeleted)
                    .Select(n => new FoodNutrientIn100gDto
                    {
                        NutrientId = n.NutrientId,
                        Amount = n.Amount
                    })
                    .ToList(),
                Measures = measures?
                    .Select(m => new MeasureDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        WeightInGrams = m.WeightInGrams
                    })
                    .ToList()
            };

            return res;
        }
    }
}
