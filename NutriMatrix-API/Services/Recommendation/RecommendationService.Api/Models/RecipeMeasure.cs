namespace RecommendationService.Api.Models
{
    public class RecipeMeasure:IEntity
    {
        public long Id { get; set; }
        public long MeasureId { get; set; }
        public long FoodId { get; set; }
        public long RecipeId { get; set; }
        public float Amount { get; set; }

        public Recipe Recipe { get; set; }
        public bool IsDeleted { get; set; }
    }
}
