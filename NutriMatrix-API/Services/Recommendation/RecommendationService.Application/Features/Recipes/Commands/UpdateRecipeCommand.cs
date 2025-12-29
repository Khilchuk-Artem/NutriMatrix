using MediatR;
using Microsoft.EntityFrameworkCore;
using RecommendationService.Application.Models.Dto;
using RecommendationService.Domain.Entities;
using RecommendationService.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationService.Application.Features.Recipes.Commands
{
    public class UpdateRecipeCommand : IRequest
    {
        public long Id { get; set; }
        public UpdateRecipeDto Dto { get; set; }
    }
    public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand>
    {
        private readonly RecipeDbContext _dbContext;

        public UpdateRecipeCommandHandler(RecipeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
        {
            var recipe = await _dbContext.Recipes
                .Include(r => r.Measures)
                .Include(r => r.NutrientsPerTotalServings)
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (recipe == null) throw new Exception("Recipe not found");

            var dto = request.Dto;
            recipe.Title = dto.Title;
            recipe.Category = dto.Category;
            recipe.Description = dto.Description;
            recipe.Directions = dto.Directions;
            recipe.PhotoUrl = dto.PhotoUrl;
            recipe.Servings = dto.Servings;

            _dbContext.RecipeMeasure.RemoveRange(recipe.Measures);
            recipe.Measures = dto.Measures.Select(m => new RecipeMeasure
            {
                MeasureId = m.MeasureId,
                FoodId = m.FoodId,
                Amount = m.Amount,
                RecipeId = recipe.Id
            }).ToList();

            _dbContext.NutrientAmounts.RemoveRange(recipe.NutrientsPerTotalServings);
            recipe.NutrientsPerTotalServings = dto.Nutrients.Select(n => new NutrientAmount
            {
                NutrientId = n.NutrientId,
                Amount = n.Amount,
                RecipeId = recipe.Id
            }).ToList();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
