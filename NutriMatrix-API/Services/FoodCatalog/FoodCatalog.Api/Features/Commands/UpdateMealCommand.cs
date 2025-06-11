using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Commands
{
    public class UpdateMealCommand : IRequest<Meal>
    {
        public long Id { get; set; }
        public UpdateMealDto UpdateMealDto { get; set; }
    }

    public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, Meal>
    {
        private readonly FoodCatalogDbContext _dbContext;

        public UpdateMealCommandHandler(FoodCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Meal> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
        {
            var meal = await _dbContext.Meals
                .Include(m => m.FoodMeals)
                .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

            if (meal == null)
            {
                return null;
            }

            var updateMealDto = request.UpdateMealDto;

            meal.Name = updateMealDto.Name;
            meal.AddedBy = updateMealDto.AddedBy;
            meal.TotalServings = updateMealDto.TotalServings;

            _dbContext.FoodMeals.RemoveRange(meal.FoodMeals);
            meal.FoodMeals = updateMealDto.FoodMeals
                .Select(fm => new FoodMeal
                {
                    MeasureId = fm.MeasureId,
                    Quantity = fm.Quantity
                })
                .ToList();

            await _dbContext.SaveChangesAsync(cancellationToken);

            return meal;
        }
    }
}
