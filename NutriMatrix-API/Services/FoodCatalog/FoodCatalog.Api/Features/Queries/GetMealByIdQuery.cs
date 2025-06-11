using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Queries
{
    public class GetMealByIdQuery : IRequest<MealDto>
    {
        public long Id { get; set; }
    }

    public class GetMealByIdQueryHandler : IRequestHandler<GetMealByIdQuery, MealDto>
    {
        private readonly FoodCatalogDbContext _dbContext;

        public GetMealByIdQueryHandler(FoodCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MealDto> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
        {
            var meal = await _dbContext.Meals
                .Where(m => m.Id == request.Id && !m.IsDeleted)
                .Select(m => new MealDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    AddedBy = m.AddedBy,
                    TotalServings = m.TotalServings,
                    FoodMeals = m.FoodMeals
                        .Select(fm => new FoodMealDto
                        {
                            Id = fm.Id,
                            MeasureId = fm.MeasureId,
                            Quantity = fm.Quantity
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return meal;
        }
    }
}
