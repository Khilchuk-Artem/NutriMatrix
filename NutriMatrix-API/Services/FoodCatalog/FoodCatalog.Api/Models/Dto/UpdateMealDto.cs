namespace FoodCatalog.Api.Models.Dto
{
    public class UpdateMealDto
    {
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public float TotalServings { get; set; }
        public IEnumerable<UpdateFoodMealDto> FoodMeals { get; set; }
    }
    public class UpdateFoodMealDto
    {
        public long Id { get; set; }
        public long MeasureId { get; set; }
        public float Quantity { get; set; }
    }
}
