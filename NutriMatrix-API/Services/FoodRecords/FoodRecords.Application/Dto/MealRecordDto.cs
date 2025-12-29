namespace FoodRecords.Application.Models.Dto
{
    public class MealRecordDto
    {
        public long Id { get; set; }
        public DateTime DateEaten { get; set; }
        public string UserId { get; set; }
        public float ServingsEaten { get; set; }
        public long MealId { get; set; }
        public IEnumerable<MealIngredientSnapshotDto> IngredientSnapshots { get; set; }
    }
}
