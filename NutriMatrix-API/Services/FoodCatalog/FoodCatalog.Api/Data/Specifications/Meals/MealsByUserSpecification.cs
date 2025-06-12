using Ardalis.Specification;
using FoodCatalog.Api.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodCatalog.Api.Data.Specifications.Meals
{
    public class MealsByUserSpecification : Specification<Meal>
    {
        public MealsByUserSpecification(string userId, string? searchQuery)
        {
            Query.Where(m => m.AddedBy == userId);
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                Query.Where(m => m.Name.ToLower().Contains(searchQuery.ToLower()));
            }
            Query.Include(m => m.FoodMeals);
        }
    }
}
