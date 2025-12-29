namespace FoodCatalog.Application.Dto
{
    public class UpdateMealDto
    {
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public float TotalServings { get; set; }
        public IEnumerable<UpdateFoodMealDto> FoodMeals { get; set; }
    }
}
