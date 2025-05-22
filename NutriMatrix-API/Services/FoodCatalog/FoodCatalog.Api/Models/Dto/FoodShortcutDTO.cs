namespace FoodCatalog.Api.Models.Dto
{
    public class FoodShortcutDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FoodNutrientIn100gDto> Nutrients { get; set; }
    }
}
