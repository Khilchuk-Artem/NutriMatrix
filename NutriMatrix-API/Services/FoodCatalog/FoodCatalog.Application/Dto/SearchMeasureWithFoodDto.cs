namespace FoodCatalog.Application.Dto
{
    public class SearchMeasureWithFoodDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public double WeightInGrams { get; set; }
        public double Quantity { get; set; }
        public FoodDTO Food { get; set; } = null!;
    }
}
