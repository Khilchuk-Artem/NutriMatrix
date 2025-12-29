using Ardalis.Specification;
using FoodCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Persistance.Specifications.Foods
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
