namespace FoodCatalog.Application.Dto
{
    public class CreateMealDto
    {
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public float TotalServings { get; set; }
        public IEnumerable<FoodMealDto> FoodMeals { get; set; }
    }
}
