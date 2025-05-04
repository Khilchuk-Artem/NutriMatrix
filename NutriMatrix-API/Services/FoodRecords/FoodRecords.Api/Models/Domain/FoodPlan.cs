using System.ComponentModel.DataAnnotations.Schema;

namespace FoodRecords.Api.Models.Domain
{
    public class FoodPlan
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }

        public Guid FoodMeasureId { get; set; }
        public double Quantity { get; set; }

        public DateTime ScheduledTime { get; set; }
        public string RecurringDaysRaw { get; set; }
        public bool RequireConfirmationOnAdd { get; set; }



        [NotMapped]
        public IEnumerable<DayOfWeek> IsRecurring
        {
            get => string.IsNullOrEmpty(RecurringDaysRaw)
                ? new List<DayOfWeek>()
                : RecurringDaysRaw.Split(',')
                    .Select(d => Enum.Parse<DayOfWeek>(d));
            set => RecurringDaysRaw = string.Join(",", value.Select(d => d.ToString()));
        }
    }
}
