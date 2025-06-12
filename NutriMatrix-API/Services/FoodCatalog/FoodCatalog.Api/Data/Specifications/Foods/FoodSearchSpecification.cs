using Ardalis.Specification;
using FoodCatalog.Api.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodCatalog.Api.Data.Specifications.Foods
{
    public class FoodSearchSpecification : Specification<Food>
    {
        public FoodSearchSpecification(string searchQuery)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                Query.Where(f => f.Name.ToLower().Contains(searchQuery.ToLower()) && !f.IsDeleted);
            }
            Query.Where(f => !f.IsDeleted);
        }
    }
}
