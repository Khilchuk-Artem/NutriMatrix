using MediatR;
using RecommendationService.Api.Data.Repository;
using RecommendationService.Api.Data.Specifications.Recipes;
using RecommendationService.Api.Models;
using RecommendationService.Api.Models.Dto;
using static RecommendationService.Api.Controllers.RecipeController;
using RecipeShortcutDto = RecommendationService.Api.Controllers.RecipeController.RecipeShortcutDto;

namespace RecommendationService.Api.Features.Recipes.Queries
{
        public class GetRecipeShortcutByIdQuery : IRequest<RecipeShortcutDto>
        {
            public long Id { get; set; }
            public long[]? NutrientIds { get; set; }
        }
        public class GetRecipeShortcutByIdQueryHandler : IRequestHandler<GetRecipeShortcutByIdQuery, RecipeShortcutDto>
        {
            private readonly IRepository<Recipe> _repository;

            public GetRecipeShortcutByIdQueryHandler(IRepository<Recipe> repository)
            {
                _repository = repository;
            }

            public async Task<RecipeShortcutDto> Handle(GetRecipeShortcutByIdQuery request, CancellationToken cancellationToken)
            {
                var spec = new RecipeByIdSpecification(request.Id);
                var recipe = await _repository.Get(request.Id, spec);
                if (recipe == null) return null;

                return ProjectToDto(recipe, request.NutrientIds);
            }

            private RecipeShortcutDto ProjectToDto(Recipe r, long[]? nutrientIds)
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

                return new RecipeShortcutDto
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
        }
    }
