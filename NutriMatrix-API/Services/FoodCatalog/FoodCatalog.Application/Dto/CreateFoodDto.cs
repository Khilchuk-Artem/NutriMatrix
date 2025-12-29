using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Application.Dto
{
    public class CreateFoodDto
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public string? Barcode { get; set; }
        public List<CreateMeasureDto> Measures { get; set; }
        public List<CreateFoodNutrientIn100gDto> Nutrients { get; set; }
    }
}
