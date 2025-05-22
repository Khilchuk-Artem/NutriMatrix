namespace FoodRecords.Api.Models.Domain
{
    public class IngredientSnapshot
    {
        public long Id { get; set; }
        public long FoodMeasureId { get; set; }
        public float Amount { get; set; }
    }
}