namespace FoodRecords.Api.Models.Dto
{
    public class AddFoodRecordDto
    {
        public DateOnly DateEaten { get; set; }
        public string UserId { get; set; }
        public Guid FoodMeasureId { get; set; }
        public float Amount { get; set; }
    }
}
