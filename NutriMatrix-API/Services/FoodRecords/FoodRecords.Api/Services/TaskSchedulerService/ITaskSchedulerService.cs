using FoodRecords.Api.Models.Domain;

namespace FoodRecords.Api.Services.TaskSchedulerService
{
    public interface ITaskSchedulerService
    {
        Task<FoodPlan> CreateScheduleAsync(ScheduleDto dto);
        Task<FoodPlan> UpdateScheduleAsync(ScheduleDto dto);
        Task DeleteScheduleAsync(long scheduleId);
    }
}