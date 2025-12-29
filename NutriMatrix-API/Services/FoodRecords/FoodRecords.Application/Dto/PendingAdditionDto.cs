using FoodRecords.Domain.Entities;

namespace FoodRecords.Application.Dto
{
    public class PendingAdditionDto
    {
        public ConsumableType ConsumableType { get; set; }
        public float Amount { get; set; }
        public string UserId { get; set; }
        public long ConsumableId { get; set; }
        public DateTime DatePending { get; set; }
    }
}
