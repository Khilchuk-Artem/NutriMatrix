namespace FoodRecords.Application.Models.Dto
{
    public class NutrientAmountDto
    {
        public Guid NutrientId { get; set; }
        public string Name { get; set; }
        public float Amount { get; set; }
    }
}
