namespace FoodCatalog.Api.Models.Dto
{
    public class MealDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public float TotalServings { get; set; }
        public IEnumerable<FoodMealDto> FoodMeals { get; set; }
    }
}
