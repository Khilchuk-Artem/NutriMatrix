using FoodCatalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Domain.Entities
{
    public class Food : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public bool IsDeleted { get; set; }
        public string? Barcode { get; set; }

        public ICollection<Measure> Measures { get; set; }
        public ICollection<FoodNutrientIn100g> FoodNutrients { get; set; }
    }
}
