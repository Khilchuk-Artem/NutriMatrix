namespace FoodCatalog.Application.Dto
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
}
