namespace FoodRecords.Api.Models.Domain
{
    public enum ConsumableType
    {
        Food,
        Recipe
    }
    public class PendingRecord:IEntity
    {
        public long Id { get; set; }
        public ConsumableType ConsumableType { get; set; }
        public long ConsumableId { get; set; }
        public float Amount { get; set; }
        public string UserId { get; set; }
        public DateTime DatePending { get; set; }
        public bool IsDeleted { get ; set; }

    }
}
