using Ardalis.Specification;
using RecommendationService.Domain.Entities;

namespace RecommendationService.Persistance.Specifications.Recipes
{
    public class RecipesWithFiltersSpecification : Specification<Recipe>
    {
        public RecipesWithFiltersSpecification(string? category, string? query, long[]? includeIngredients, long[]? excludeIngredients)
        {
            Query.Where(r => !r.IsDeleted && r.Measures.Count != 0)
                 .Include(r => r.NutrientsPerTotalServings)
                 .Include(r => r.Measures);

            if (!string.IsNullOrWhiteSpace(category))
                Query.Where(r => r.Category == category);

            if (!string.IsNullOrWhiteSpace(query))
                Query.Where(r => r.Title.Contains(query));

            if (includeIngredients?.Length > 0)
                Query.Where(r => includeIngredients.All(ingId => r.Measures.Any(m => m.FoodId == ingId)));

            if (excludeIngredients?.Length > 0)
                Query.Where(r => r.Measures.All(m => !excludeIngredients.Contains(m.FoodId)));
        }
    }
}
