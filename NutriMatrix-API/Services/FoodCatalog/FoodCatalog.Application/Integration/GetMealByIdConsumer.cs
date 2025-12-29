using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Responses;
using FoodCatalog.Persistance.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Application.Integration
{
    public class GetMealByIdConsumer : IConsumer<GetMealByIdRequest>
    {
        private readonly FoodCatalogDbContext _context;
        public GetMealByIdConsumer(FoodCatalogDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<GetMealByIdRequest> context)
        {
            var meal = await _context.Meals
                .Include(m => m.FoodMeals)
                    .ThenInclude(fm => fm.Measure)
                .FirstOrDefaultAsync(m => m.Id == context.Message.Id && !m.IsDeleted);

            if (meal is null)
            {
                throw new InvalidOperationException($"Meal with ID {context.Message.Id} not found");
            }

            var dto = new MealResponseDto(
                meal.Id,
                meal.Name,
                meal.AddedBy,
                meal.TotalServings,
                meal.FoodMeals
                    .Where(fm => !fm.IsDeleted)
                    .Select(fm => new FoodMealResponseDto(
                        fm.Id,
                        fm.MeasureId,
                        fm.Quantity,
                        fm.Measure?.Name ?? "",
                        fm.Measure?.WeightInGrams ?? 0f
                    )).ToList()
            );

            await context.RespondAsync(dto);
        }
    }
}
