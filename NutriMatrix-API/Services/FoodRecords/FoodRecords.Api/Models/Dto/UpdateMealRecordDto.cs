namespace FoodRecords.Api.Models.Dto
{
    public class UpdateMealRecordDto
    {
        public long MealId { get; set; }
        public DateTime DateEaten { get; set; }
        public float ServingsEaten { get; set; }
        public List<UpdateMealIngredientSnapshotDto> IngredientSnapshots { get; set; }
    }
}
