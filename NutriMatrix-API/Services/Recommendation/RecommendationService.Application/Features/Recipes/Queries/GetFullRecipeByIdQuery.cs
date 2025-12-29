using MediatR;
using RecommendationService.Application.Models.Dto;
using RecommendationService.Domain.Contracts;
using RecommendationService.Domain.Entities;
using RecommendationService.Persistance.Specifications.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationService.Application.Features.Recipes.Queries
{
    public class GetFullRecipeByIdQuery : IRequest<FullRecipeDto>
    {
        public long Id { get; set; }
        public string? NutrientIds { get; set; }
    }
    public class GetFullRecipeByIdQueryHandler : IRequestHandler<GetFullRecipeByIdQuery, FullRecipeDto>
    {
        private readonly IRepository<Recipe> _repository;

        public GetFullRecipeByIdQueryHandler(IRepository<Recipe> repository)
        {
            _repository = repository;
        }

        public async Task<FullRecipeDto> Handle(GetFullRecipeByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new RecipeByIdSpecification(request.Id);
            var recipe = await _repository.Get(request.Id, spec);
            if (recipe == null) return null;

            long[]? nutrientIdsArray = ParseNutrientIds(request.NutrientIds);
            var nutrients = nutrientIdsArray?.Length > 0
                ? recipe.NutrientsPerTotalServings.Where(n => nutrientIdsArray.Contains(n.NutrientId)).ToList()
                : recipe.NutrientsPerTotalServings;

            return new FullRecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Category = recipe.Category,
                Servings = recipe.Servings ?? 0,
                Description = recipe.Description,
                Directions = recipe.Directions,
                PhotoUrl = recipe.PhotoUrl,
                Ingredients = recipe.Measures.Select(m => new IngredientMeasureDto
                {
                    FoodId = m.FoodId,
                    MeasureId = m.MeasureId,
                    Amount = m.Amount
                }).ToList(),
                Nutrients = nutrients.Select(n => new NutrientAmountDto
                {
                    NutrientId = n.NutrientId,
                    Amount = n.Amount
                }).ToList()
            };
        }
        private static long[]? ParseNutrientIds(string? nutrientIds)
        {
            if (string.IsNullOrWhiteSpace(nutrientIds))
                return null;

            var parts = nutrientIds.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            var list = new List<long>();

            foreach (var part in parts)
            {
                if (long.TryParse(part, out var val))
                    list.Add(val);
            }

            return list.Count > 0 ? list.ToArray() : null;
        }
    }
}
