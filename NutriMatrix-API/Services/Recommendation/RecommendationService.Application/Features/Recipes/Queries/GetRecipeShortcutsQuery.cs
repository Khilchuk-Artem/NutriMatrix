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
    public class GetRecipeShortcutsQuery : IRequest<List<FullRecipeShortcutDto>>
    {
        public string? Category { get; set; }
        public string? Query { get; set; }
        public string? NutrientIds { get; set; }
        public string? IncludeIngredients { get; set; }
        public string? ExcludeIngredients { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class GetRecipeShortcutsQueryHandler : IRequestHandler<GetRecipeShortcutsQuery, List<FullRecipeShortcutDto>>
    {
        private readonly IRepository<Recipe> _repository;

        public GetRecipeShortcutsQueryHandler(IRepository<Recipe> repository)
        {
            _repository = repository;
        }

        public async Task<List<FullRecipeShortcutDto>> Handle(GetRecipeShortcutsQuery request, CancellationToken cancellationToken)
        {
            long[]? include = ParseNutrientIds(request.IncludeIngredients);
            long[]? exclude = ParseNutrientIds(request.ExcludeIngredients);
            long[]? nutrientIdsArray = ParseNutrientIds(request.NutrientIds);

            var spec = new RecipesWithFiltersSpecification(request.Category, request.Query, include, exclude);
            var recipes = await _repository.GetAll(spec, request.Page, request.PageSize);
            return recipes.Select(r => ProjectToDto(r, nutrientIdsArray)).ToList();
        }
        private FullRecipeShortcutDto ProjectToDto(Recipe r, long[]? nutrientIds)
        {
            var filteredNutrients = nutrientIds?.Length > 0
                ? r.NutrientsPerTotalServings
                    .Where(n => nutrientIds.Contains(n.NutrientId))
                    .Select(n => new NutrientAmountDto { NutrientId = n.NutrientId, Amount = n.Amount })
                    .ToList()
                : r.NutrientsPerTotalServings
                    .Select(n => new NutrientAmountDto { NutrientId = n.NutrientId, Amount = n.Amount })
                    .ToList();

            var ingredients = r.Measures.Select(m => new IngredientMeasureDto
            {
                Amount = m.Amount,
                FoodId = m.FoodId,
                MeasureId = m.MeasureId,
            }).ToList();

            return new FullRecipeShortcutDto
            {
                Id = r.Id,
                Title = r.Title,
                RecipeId = r.Id,
                Servings = r.Servings ?? 0,
                Category = r.Category,
                Ingredients = ingredients,
                Nutrients = filteredNutrients
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
