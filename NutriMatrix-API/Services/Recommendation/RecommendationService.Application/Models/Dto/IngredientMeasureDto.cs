namespace RecommendationService.Application.Models.Dto
{
    public class IngredientMeasureDto
    {
        public float Amount { get; set; }
        public long FoodId { get; set; }
        public long MeasureId { get; set; }
    }
}
