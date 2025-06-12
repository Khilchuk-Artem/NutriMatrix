using FoodRecords.Api.Models.Domain;

namespace FoodRecords.Api.Services.TaskSchedulerService
{
    public class ScheduleDto
    {
        public long? Id { get; set; }
        public long ConsumableId { get; set; }
        public float Amount { get; set; }
        public bool RequiresConfirmation { get; set; }
        public string Name { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime? RunAtUtc { get; set; }
        public string? CronExpression { get; set; }
        public string UserId { get; set; }
        public ConsumableType ConsumableType { get; set; }
    }
}
