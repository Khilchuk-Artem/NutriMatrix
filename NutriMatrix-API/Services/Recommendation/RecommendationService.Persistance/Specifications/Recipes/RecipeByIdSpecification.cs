using Ardalis.Specification;
using RecommendationService.Domain.Entities;

namespace RecommendationService.Persistance.Specifications.Recipes
{
    public class RecipeByIdSpecification : Specification<Recipe>
    {
        public RecipeByIdSpecification(long id)
        {
            Query
                 .Include(r => r.NutrientsPerTotalServings)
                 .Include(r => r.Measures);
        }
    }
}
