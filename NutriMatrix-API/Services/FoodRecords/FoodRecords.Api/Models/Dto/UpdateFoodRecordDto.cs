namespace FoodRecords.Api.Models.Dto
{
    public class UpdateFoodRecordDto
    {
        public Guid FoodMeasureId { get; set; }
        public float Amount { get; set; }
    }
}
