namespace FoodCatalog.Api.Models.Domain
{
    public class FoodMeal:IEntity
    {
        public long Id { get; set; }
        public long MeasureId { get; set; }
        public long MealId { get; set; }
        public float Quantity { get; set; }
        public bool IsDeleted { get; set; }

        public Measure Measure { get; set; }
    }
}
