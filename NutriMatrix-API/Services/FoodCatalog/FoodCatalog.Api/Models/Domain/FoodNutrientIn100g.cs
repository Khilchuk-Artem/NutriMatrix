namespace FoodCatalog.Api.Models.Domain
{
    public class FoodNutrientIn100g:IEntity
    {
        public Guid Id { get; set; }
        public Guid FoodId { get; set; }
        public Guid NutrientId { get; set; }
        public float Amount { get; set; }
        public bool IsDeleted { get; set; }

        public Food Food { get; set; }
        public Nutrient Nutrient { get; set; }
    }
}
