using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;
using static FoodCatalog.Api.Controllers.FoodController;

namespace FoodCatalog.Api.Features.Commands
{
    public class CreateFoodCommand : IRequest<Food>
    {
        public CreateFoodDto CreateFoodDto { get; set; }
    }

    public class CreateFoodCommandHandler : IRequestHandler<CreateFoodCommand, Food>
    {
        private readonly FoodCatalogDbContext _dbContext;
        private readonly RedisCollection<FoodRedis> _foodCollection;
        private readonly RedisCollection<MeasureRedis> _measureCollection;

        public CreateFoodCommandHandler(FoodCatalogDbContext dbContext, RedisCollection<FoodRedis> foodCollection, RedisCollection<MeasureRedis> measureCollection)
        {
            _dbContext = dbContext;
            _foodCollection = foodCollection;
            _measureCollection = measureCollection;
        }

        public async Task<Food> Handle(CreateFoodCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CreateFoodDto;
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Food name cannot be empty.");
            }

            long currentMaxFoodId = await _dbContext.Foods.AnyAsync() ? await _dbContext.Foods.MaxAsync(f => f.Id) : 0;
            long currentMaxMeasureId = await _dbContext.Measures.AnyAsync() ? await _dbContext.Measures.MaxAsync(m => m.Id) : 0;
            long currentMaxNutrientId = await _dbContext.FoodNutrientIn100Gs.AnyAsync() ? await _dbContext.FoodNutrientIn100Gs.MaxAsync(n => n.Id) : 0;

            currentMaxFoodId++;
            var food = new Food
            {
                Id = currentMaxFoodId,
                Name = dto.Name,
                Photo = dto.Photo,
                Barcode = dto.Barcode,
                IsDeleted = false,
                Measures = new List<Measure>(),
                FoodNutrients = new List<FoodNutrientIn100g>()
            };

            foreach (var measureDto in dto.Measures ?? new List<CreateMeasureDto>())
            {
                currentMaxMeasureId++;
                var measure = new Measure
                {
                    Id = currentMaxMeasureId,
                    Name = measureDto.Name,
                    WeightInGrams = measureDto.WeightInGrams,
                    FoodId = food.Id,
                    IsDeleted = false
                };
                food.Measures.Add(measure);
            }

            foreach (var nutrientDto in dto.Nutrients ?? new List<CreateFoodNutrientIn100gDto>())
            {
                currentMaxNutrientId++;
                var nutrient = new FoodNutrientIn100g
                {
                    Id = currentMaxNutrientId,
                    NutrientId = nutrientDto.NutrientId,
                    Amount = nutrientDto.Amount,
                    FoodId = food.Id,
                    IsDeleted = false
                };
                food.FoodNutrients.Add(nutrient);
            }

            _dbContext.Foods.Add(food);
            await _dbContext.SaveChangesAsync();

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

            return food;
        }
    }
}
