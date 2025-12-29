using Ardalis.Specification;
using FoodCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Persistance.Specifications.Meals
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
