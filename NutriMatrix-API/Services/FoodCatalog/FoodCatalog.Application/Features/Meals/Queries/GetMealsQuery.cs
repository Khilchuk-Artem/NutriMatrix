using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Specifications.Meals;
using MediatR;

namespace FoodCatalog.Api.Features.Meals.Queries
{
    public class GetMealsQuery : IRequest<List<MealDto>>
    {
        public string UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchQuery { get; set; }
    }

    public class GetMealsQueryHandler : IRequestHandler<GetMealsQuery, List<MealDto>>
    {
        private readonly IRepository<Meal> _mealRepository;

        public GetMealsQueryHandler(IRepository<Meal> mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public async Task<List<MealDto>> Handle(GetMealsQuery request, CancellationToken cancellationToken)
        {
            var spec = new MealsByUserSpecification(request.UserId, request.SearchQuery);
            var meals = await _mealRepository.GetAll(spec, request.PageNumber, request.PageSize);

            return meals.Select(m => new MealDto
            {
                Id = m.Id,
                Name = m.Name,
                AddedBy = m.AddedBy,
                TotalServings = m.TotalServings,
                FoodMeals = m.FoodMeals
                    .Select(fm => new FoodMealDto
                    {
                        MeasureId = fm.MeasureId,
                        Quantity = fm.Quantity
                    })
                    .ToList()
            }).ToList();
        }
    }
}