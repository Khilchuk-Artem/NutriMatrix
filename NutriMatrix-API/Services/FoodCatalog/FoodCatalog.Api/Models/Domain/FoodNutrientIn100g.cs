namespace FoodCatalog.Api.Models.Domain
{
    public class FoodNutrientIn100g:IEntity
    {
        public long Id { get; set; }
        public long FoodId { get; set; }
        public long NutrientId { get; set; }
        public float Amount { get; set; }
        public bool IsDeleted { get; set; }

        public Food Food { get; set; }
        public Nutrient Nutrient { get; set; }
    }
}
