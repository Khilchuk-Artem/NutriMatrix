using System.ComponentModel.DataAnnotations.Schema;
using FoodRecords.Domain.Common;

namespace FoodRecords.Domain.Entities
{
    public class FoodPlan:IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ConsumableId { get; set; }
        public float Amount { get; set; }
        public string UserId { get; set; }
        public bool RequiresConfirmation { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime? RunAtUtc { get; set; }
        public string? CronExpression { get; set; }
        public string JobKey { get; set; }
        public string TriggerKey { get; set; }
        public ConsumableType ConsumableType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
