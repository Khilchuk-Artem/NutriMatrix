namespace FoodRecords.Api.Models.Domain
{
    public class MealRecord
    {
        public long Id { get; set; }
        public DateTime DateEaten { get; set; }
        public string UserId { get; set; }
        public float ServingsEaten { get; set; }
        public long MealId { get; set; }

        public IEnumerable<MealIngredientSnapshot> IngredientSnapshots { get; set; }
        public bool IsDeleted { get; set; }
    }
}
    