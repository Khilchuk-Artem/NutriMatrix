using FoodCatalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Domain.Entities
{
    public class Meal : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public bool IsDeleted { get; set; }
        public float TotalServings { get; set; }
        public ICollection<FoodMeal> FoodMeals { get; set; }

    }
}
