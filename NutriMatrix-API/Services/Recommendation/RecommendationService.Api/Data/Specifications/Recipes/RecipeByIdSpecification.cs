using Ardalis.Specification;
using RecommendationService.Api.Models;

namespace RecommendationService.Api.Data.Specifications.Recipes
{
    public class RecipeByIdSpecification : Specification<Recipe>
    {
        public RecipeByIdSpecification(long id)
        {
            Query.Where(r => r.Id == id && !r.IsDeleted)
                 .Include(r => r.NutrientsPerTotalServings)
                 .Include(r => r.Measures);
        }
    }
}
