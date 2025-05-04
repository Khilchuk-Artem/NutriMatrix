namespace FoodRecords.Api.Models.Dto
{
    public class FoodRecordDto
    {
        public Guid RecordId { get; set; }
        public Guid FoodMeasureId { get; set; }
        public string FoodName { get; set; }
        public string MeasureName { get; set; }
        public float MeasureWeightInGrams { get; set; } 
        public float Amount { get; set; }

        public List<NutrientAmountDto> Nutrients { get; set; } = new();
    }
}
