namespace RecommendationService.Api.Models
{
    public class RecipeMeasure:IEntity
    {
        public Guid Id { get; set; }
        public Guid MeasureId { get; set; }
        public Guid FoodId { get; set; }
        public Guid RecipeId { get; set; }
        public float Amount { get; set; }

        public Recipe Recipe { get; set; }
        public bool IsDeleted { get; set; }
    }
}
