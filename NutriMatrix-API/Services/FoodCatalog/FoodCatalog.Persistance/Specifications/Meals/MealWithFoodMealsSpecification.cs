using Ardalis.Specification;
using FoodCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodCatalog.Persistance.Specifications.Meals
{
    public class MealWithFoodMealsSpecification : Specification<Meal>
    {
        public MealWithFoodMealsSpecification()
        {
            Query.Include(m => m.FoodMeals);
        }
    }
}
