using Ardalis.Specification;
using FoodCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Persistance.Specifications.Foods
{
    public class FoodByIdSpecification : Specification<Food>
    {
        public FoodByIdSpecification(long id)
        {
            Query.Where(f => f.Id == id && !f.IsDeleted)
                 .Include(f => f.FoodNutrients)
                 .Include(f => f.Measures);
        }
    }
}
