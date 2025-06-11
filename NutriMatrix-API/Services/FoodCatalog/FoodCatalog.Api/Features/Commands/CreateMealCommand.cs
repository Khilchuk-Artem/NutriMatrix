using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Commands
{
    public class CreateMealCommand : IRequest<MealDto>
    {
        public CreateMealDto CreateMealDto { get; set; }
    }

    public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, MealDto>
    {
        private readonly FoodCatalogDbContext _dbContext;

        public CreateMealCommandHandler(FoodCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MealDto> Handle(CreateMealCommand request, CancellationToken cancellationToken)
        {
            var createMealDto = request.CreateMealDto;

            foreach (var fm in createMealDto.FoodMeals)
            {
                var measure = await _dbContext.Measures
                    .FirstOrDefaultAsync(ms => ms.Id == fm.MeasureId, cancellationToken);
                if (measure == null)
                {
                    throw new Exception($"Measure with ID {fm.MeasureId} not found.");
                }
            }

            var meal = new Meal
            {
                Name = createMealDto.Name,
                AddedBy = createMealDto.AddedBy,
                TotalServings = createMealDto.TotalServings,
                IsDeleted = false,
                FoodMeals = createMealDto.FoodMeals
                    .Select(fm => new FoodMeal
                    {
                        MeasureId = fm.MeasureId,
                        Quantity = fm.Quantity
                    })
                    .ToList()
            };

            _dbContext.Meals.Add(meal);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var mealDto = new MealDto
            {
                Id = meal.Id,
                Name = meal.Name,
                AddedBy = meal.AddedBy,
                TotalServings = meal.TotalServings,
                FoodMeals = meal.FoodMeals
                    .Select(fm => new FoodMealDto
                    {
                        MeasureId = fm.MeasureId,
                        Quantity = fm.Quantity
                    })
                    .ToList()
            };

            return mealDto;
        }
    }
}
