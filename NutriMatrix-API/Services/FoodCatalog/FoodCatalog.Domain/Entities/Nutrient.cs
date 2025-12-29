using FoodCatalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Domain.Entities
{
    public class Nutrient : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool IsDeleted { get; set; }
    }
}
