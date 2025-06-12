using Ardalis.Specification;
using FoodCatalog.Api.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodCatalog.Api.Data.Specifications.Foods
{
    public class FoodByBarcodeSpecification : Specification<Food>
    {
        public FoodByBarcodeSpecification(string barcode)
        {
            Query.Where(f => f.Barcode == barcode && !f.IsDeleted)
                 .Include(f => f.FoodNutrients)
                 .Include(f => f.Measures);
        }
    }
}
