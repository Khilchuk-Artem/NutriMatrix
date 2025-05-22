namespace FoodRecords.Api.Models.Dto
{
    public class AddFoodRecordDto
    {
        public DateTime DateEaten { get; set; }
        public string UserId { get; set; }
        public long FoodMeasureId { get; set; }
        public float Amount { get; set; }
    }
}
