namespace FoodCatalog.Api.Models.Dto
{
    public class FoodDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Barcode { get; set; }
        public IEnumerable<FoodNutrientIn100gDto> FoodNutrients { get; set; }
        public IEnumerable<MeasureDto> Measures { get; set; }
    }
    public class FoodNutrientIn100gDto
    {
        public long NutrientId { get; set; }
        public float Amount { get; set; }
    }
    public class MeasureDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float WeightInGrams { get; set; }
    }
}
