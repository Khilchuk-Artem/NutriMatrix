using Ardalis.Specification;
using FoodCatalog.Api.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodCatalog.Api.Data.Specifications.Meals
{
    public class MealWithFoodMealsSpecification : Specification<Meal>
    {
        public MealWithFoodMealsSpecification()
        {
            Query.Include(m => m.FoodMeals);
        }
    }
}
