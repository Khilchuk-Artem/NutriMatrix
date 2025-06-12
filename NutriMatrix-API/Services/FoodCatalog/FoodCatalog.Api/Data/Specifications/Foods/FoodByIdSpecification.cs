using Ardalis.Specification;
using FoodCatalog.Api.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodCatalog.Api.Data.Specifications.Foods
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
