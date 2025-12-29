using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Specifications.Meals;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Meals.Commands
{
    public class UpdateMealCommand : IRequest<Meal>
    {
        public long Id { get; set; }
        public UpdateMealDto UpdateMealDto { get; set; }
    }

    public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, Meal>
    {
        private readonly IRepository<Meal> _mealRepository;

        public UpdateMealCommandHandler(IRepository<Meal> mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public async Task<Meal> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
        {
            var spec = new MealWithFoodMealsSpecification();
            var meal = await _mealRepository.Get(request.Id, spec);
            if (meal == null)
            {
                return null;
            }

            var updateMealDto = request.UpdateMealDto;

            meal.Name = updateMealDto.Name;
            meal.AddedBy = updateMealDto.AddedBy;
            meal.TotalServings = updateMealDto.TotalServings;

            meal.FoodMeals.Clear();
            var foodMeals = updateMealDto.FoodMeals
                .Select(fm => new FoodMeal
                {
                    MeasureId = fm.MeasureId,
                    Quantity = fm.Quantity
                });

            foreach(var a in foodMeals)
            {
                meal.FoodMeals.Add(a);
            }
            
            await _mealRepository.Update(meal);

            return meal;
        }
    }
}
