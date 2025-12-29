namespace FoodRecords.Application.Models.Dto
{
    public class UpdateMealIngredientSnapshotDto
    {
        public long Id { get; set; }
        public long FoodMeasureId { get; set; }
        public float Amount { get; set; }
    }
}