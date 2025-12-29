using MediatR;
using RecommendationService.Application.Models.Dto;
using RecommendationService.Domain.Contracts;
using RecommendationService.Domain.Entities;
using RecommendationService.Persistance.Specifications.Recipes;
namespace RecommendationService.Application.Features.Recipes.Queries
{
    public class GetRecipeShortcutByIdQuery : IRequest<FullRecipeShortcutDto>
    {
        public long Id { get; set; }
        public string? NutrientIds { get; set; }
    }
    public class GetRecipeShortcutByIdQueryHandler : IRequestHandler<GetRecipeShortcutByIdQuery, FullRecipeShortcutDto>
    {
        private readonly IRepository<Recipe> _repository;

        public GetRecipeShortcutByIdQueryHandler(IRepository<Recipe> repository)
        {
            _repository = repository;
        }

        public async Task<FullRecipeShortcutDto> Handle(GetRecipeShortcutByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new RecipeByIdSpecification(request.Id);
            var recipe = await _repository.Get(request.Id, spec);
            if (recipe == null) return null;

            return ProjectToDto(recipe, ParseNutrientIds(request.NutrientIds));
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
