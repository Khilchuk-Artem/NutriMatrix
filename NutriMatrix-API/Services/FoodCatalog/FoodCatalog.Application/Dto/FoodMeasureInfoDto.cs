namespace FoodCatalog.Application.Dto
{
    public class FoodMeasureInfoDto
    {
        public Guid Id;
        public string FoodName { get; set; }
        public string MeasureName { get; set; }
        public float WeightInG { get; set; }
        public IEnumerable<NutrientAmountDto> NutrientsIn100G { get; set; }
    }
}
