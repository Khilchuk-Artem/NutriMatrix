using FoodCatalog.Api.Models.Dto;

namespace FoodCatalog.Api.Controllers
{
    namespace FoodCatalog.Api.Models.Dto
    {
        public class MeasureWithFoodDto
        {
            public long Id { get; set; }
            public string Name { get; set; } = null!;
            public double WeightInGrams { get; set; }

            public FoodShortcutDTO Food { get; set; } = null!;
        }
    }
}
