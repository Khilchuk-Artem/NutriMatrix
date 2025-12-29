using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Meals.Commands
{
    public class CreateMealCommand : IRequest<MealDto>
    {
        public CreateMealDto CreateMealDto { get; set; }
    }

    public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, MealDto>
    {
        private readonly IRepository<Meal> _mealRepository;
        private readonly IRepository<Measure> _measureRepository;

        public CreateMealCommandHandler(IRepository<Meal> mealRepository, IRepository<Measure> measureRepository)
        {
            _mealRepository = mealRepository;
            _measureRepository = measureRepository;
        }

        public async Task<MealDto> Handle(CreateMealCommand request, CancellationToken cancellationToken)
        {
            var createMealDto = request.CreateMealDto;

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

            await _mealRepository.Add(meal);

            return new MealDto
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
        }
    }
}
