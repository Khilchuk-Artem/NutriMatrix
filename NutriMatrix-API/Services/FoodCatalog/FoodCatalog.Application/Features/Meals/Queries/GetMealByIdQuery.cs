using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Specifications.Meals;
using MediatR;

namespace FoodCatalog.Api.Features.Meals.Queries
{
    public class GetMealByIdQuery : IRequest<MealDto>
    {
        public long Id { get; set; }
    }

    public class GetMealByIdQueryHandler : IRequestHandler<GetMealByIdQuery, MealDto>
    {
        private readonly IRepository<Meal> _mealRepository;

        public GetMealByIdQueryHandler(IRepository<Meal> mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public async Task<MealDto> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new MealWithFoodMealsSpecification();
            var meal = await _mealRepository.Get(request.Id, spec);
            if (meal == null)
            {
                return null;
            }

            return new MealDto
            {
                Id = meal.Id,
                Name = meal.Name,
                AddedBy = meal.AddedBy,
                TotalServings = meal.TotalServings,
                FoodMeals = meal.FoodMeals
                    .Select(fm => new FoodMealDto
                    {
                        Id = fm.Id,
                        MeasureId = fm.MeasureId,
                        Quantity = fm.Quantity
                    })
                    .ToList()
            };
        }
    }
}