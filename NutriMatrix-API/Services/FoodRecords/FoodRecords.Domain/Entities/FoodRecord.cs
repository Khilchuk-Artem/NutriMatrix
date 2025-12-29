
using FoodRecords.Domain.Common;

namespace FoodRecords.Domain.Entities
{
    public class FoodRecord : IEntity
    {
        public long Id { get; set; }
        public DateTime DateEaten { get; set; }
        public string UserId { get; set; }
        public long FoodMeasureId { get; set; }
        public float Amount { get; set; }

        public bool IsDeleted { get; set; }
    }
}
