namespace FoodRecords.Application.Models.Dto
{
    public class MealIngredientSnapshotDto
    {
        public long Id { get; set; }
        public long FoodMeasureId { get; set; }
        public float Amount { get; set; }
    }
}
