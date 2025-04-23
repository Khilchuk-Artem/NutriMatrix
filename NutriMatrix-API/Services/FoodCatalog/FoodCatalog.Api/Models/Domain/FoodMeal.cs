namespace FoodCatalog.Api.Models.Domain
{
    public class FoodMeal:IEntity
    {
        public Guid Id { get; set; }
        public Guid MeasureId { get; set; }
        public Guid MealId { get; set; }
        public float Quantity { get; set; }
        public bool IsDeleted { get; set; }

        public Measure Measure { get; set; }
    }
}
