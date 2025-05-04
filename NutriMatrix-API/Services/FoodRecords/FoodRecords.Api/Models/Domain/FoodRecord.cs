
namespace FoodRecords.Api.Models.Domain
{
    public class FoodRecord : IEntity
    {
        public Guid Id { get; set; }
        public DateOnly DateEaten { get; set; }
        public string UserId { get; set; }
        public Guid FoodMeasureId { get; set; }
        public float Amount { get; set; }

        public bool IsDeleted { get; set; }
    }
}
