namespace FoodRecords.Api.Models.Domain
{
    public class RecipeRecord
    {
        public long Id { get; set; }
        public DateTime DateEaten { get; set; }
        public string UserId { get; set; }

        public IEnumerable<IngredientSnapshot> IngredientSnapshots { get; set; }
        public bool IsDeleted { get; set; }
    }
}
