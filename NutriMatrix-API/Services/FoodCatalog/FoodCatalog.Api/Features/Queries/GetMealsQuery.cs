using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Queries
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
        private readonly FoodCatalogDbContext _dbContext;

        public GetMealsQueryHandler(FoodCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MealDto>> Handle(GetMealsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Meals
                .Where(m => !m.IsDeleted)
                .Where(m => m.AddedBy == request.UserId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                query = query.Where(m => m.Name.ToLower().Contains(request.SearchQuery.ToLower()));
            }

            var meals = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new MealDto
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
                })
                .ToListAsync(cancellationToken);

            return meals;
        }
    }
}
