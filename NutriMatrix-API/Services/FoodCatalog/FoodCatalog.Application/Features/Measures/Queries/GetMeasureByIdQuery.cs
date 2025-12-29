
using FoodCatalog.Api.Controllers.FoodCatalog.Application.Dto;
using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Features.Measures.Queries
{
    public class GetMeasureByIdQuery : IRequest<MeasureWithFoodDto>
    {
        public long Id { get; set; }
    }

    public class GetMeasureByIdQueryHandler : IRequestHandler<GetMeasureByIdQuery, MeasureWithFoodDto>
    {
        private readonly RedisCollection<MeasureRedis> _measureCollection;
        private readonly RedisCollection<FoodRedis> _foodCollection;
        private readonly IRepository<Measure> _measureRepository;

        public GetMeasureByIdQueryHandler(
            RedisCollection<MeasureRedis> measureCollection,
            RedisCollection<FoodRedis> foodCollection,
            IRepository<Measure> measureRepository) 
        {
            _measureCollection = measureCollection;
            _foodCollection = foodCollection;
            _measureRepository = measureRepository;
        }

        public async Task<MeasureWithFoodDto> Handle(GetMeasureByIdQuery request, CancellationToken cancellationToken)
        {
            var measure = await _measureCollection.FirstOrDefaultAsync(m => m.Id == request.Id);
            if (measure == null)
            {
                var measureDomain = await _measureRepository.Get(request.Id);
                if (measureDomain == null) return null;

                measure = new MeasureRedis
                {
                    Id = measureDomain.Id,
                    Name = measureDomain.Name,
                    WeightInGrams = measureDomain.WeightInGrams,
                    FoodId = measureDomain.FoodId,
                };
            }

            var food = await _foodCollection.FirstOrDefaultAsync(f => f.Id == measure.FoodId);
            if (food == null) throw new Exception($"Food with ID {measure.FoodId} not found.");

            var dto = new MeasureWithFoodDto
            {
                Id = measure.Id,
                Name = measure.Name,
                WeightInGrams = measure.WeightInGrams,
                Food = new FoodShortcutDTO
                {
                    Id = food.Id,
                    Name = food.Name,
                    Nutrients = food.FoodNutrients?
                        .Select(n => new FoodNutrientIn100gDto
                        {
                            NutrientId = n.NutrientId,
                            Amount = n.Amount
                        })
                        .ToList() ?? new List<FoodNutrientIn100gDto>()
                }
            };

            return dto;
        }
    }
}
