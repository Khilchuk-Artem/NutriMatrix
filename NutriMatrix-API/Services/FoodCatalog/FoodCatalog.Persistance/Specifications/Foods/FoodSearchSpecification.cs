using Ardalis.Specification;
using FoodCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Persistance.Specifications.Foods
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
