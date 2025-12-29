using FoodCatalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Domain.Entities
{
    public class FoodMeal : IEntity
    {
        public long Id { get; set; }
        public long MeasureId { get; set; }
        public long MealId { get; set; }
        public float Quantity { get; set; }
        public bool IsDeleted { get; set; }

        public Measure Measure { get; set; }
        public Meal Meal { get; set; }
    }
}
