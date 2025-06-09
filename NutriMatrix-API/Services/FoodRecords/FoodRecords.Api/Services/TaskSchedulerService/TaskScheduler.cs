using FoodRecords.Api.Data;
using FoodRecords.Api.Jobs;
using FoodRecords.Api.Models.Domain;
using Quartz;

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
    public class TaskSchedulerService : ITaskSchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly FoodRecordsDbContext _dbContext;

        public TaskSchedulerService(IScheduler scheduler, FoodRecordsDbContext dbContext)
        {
            _scheduler = scheduler;
            _dbContext = dbContext;
        }

        public async Task<FoodPlan> CreateScheduleAsync(ScheduleDto dto)
        {
            if (!Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
            {
                throw new ArgumentException("ConsumableType must be 'Food' or 'Recipe'");
            }

            if (string.IsNullOrEmpty(dto.UserId))
            {
                throw new ArgumentException("UserId cannot be null or empty");
            }

            var tempId = Guid.NewGuid().ToString();

            var jobKeyStr = $"objectJob-{tempId}";
            var triggerKeyStr = $"trigger-{tempId}";

            var foodPlan = new FoodPlan
            {
                ConsumableId = dto.ConsumableId,
                Amount = dto.Amount,
                RequiresConfirmation = dto.RequiresConfirmation,
                IsRecurring = dto.IsRecurring,
                RunAtUtc = dto.IsRecurring ? null : dto.RunAtUtc,
                CronExpression = dto.IsRecurring ? dto.CronExpression : null,
                UserId = dto.UserId,
                IsDeleted = false,
                ConsumableType = dto.ConsumableType,
                JobKey = jobKeyStr,
                TriggerKey = triggerKeyStr,
                Name = dto.Name
            };

            _dbContext.FoodPlans.Add(foodPlan);
            await _dbContext.SaveChangesAsync();

            var finalJobKeyStr = $"objectJob-{foodPlan.Id}";
            var finalTriggerKeyStr = $"trigger-{foodPlan.Id}";

            foodPlan.JobKey = finalJobKeyStr;
            foodPlan.TriggerKey = finalTriggerKeyStr;
            await _dbContext.SaveChangesAsync(); 

            var jobKey = new JobKey(finalJobKeyStr);
            var triggerKey = new TriggerKey(finalTriggerKeyStr);

            var jobDetail = JobBuilder.Create(dto.ConsumableType == ConsumableType.Food ? typeof(FoodRecordCreationJob) : typeof(RecipeRecordCreationJob))
                .WithIdentity(jobKey)
                .UsingJobData(new JobDataMap
                {
                    { "ObjectId", foodPlan.ConsumableId },
                    { "Amount", (float)foodPlan.Amount },
                    { "RequiresConfirmation", foodPlan.RequiresConfirmation },
                    { "ScheduleId", foodPlan.Id },
                    { "ObjectType", foodPlan.ConsumableType }
                })
                .Build();

            ITrigger trigger = foodPlan.IsRecurring
                ? TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .WithCronSchedule(foodPlan.CronExpression, x => x.InTimeZone(TimeZoneInfo.Utc))
                    .Build()
                : TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartAt(foodPlan.RunAtUtc.Value)
                    .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);

            return foodPlan;
        }


        public async Task<FoodPlan> UpdateScheduleAsync(ScheduleDto dto)
        {
            if (dto.Id == null || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
            {
                throw new ArgumentException("Invalid Id or ConsumableType; ConsumableType must be 'Food' or 'Recipe'");
            }

            if (string.IsNullOrEmpty(dto.UserId))
            {
                throw new ArgumentException("UserId cannot be null or empty");
            }

            var foodPlan = await _dbContext.FoodPlans.FindAsync(dto.Id);
            if (foodPlan == null || foodPlan.IsDeleted)
            {
                throw new Exception("FoodPlan not found or deleted");
            }

            foodPlan.ConsumableId = dto.ConsumableId;
            foodPlan.Amount = dto.Amount;
            foodPlan.RequiresConfirmation = dto.RequiresConfirmation;
            foodPlan.IsRecurring = dto.IsRecurring;
            foodPlan.RunAtUtc = dto.IsRecurring ? null : dto.RunAtUtc;
            foodPlan.CronExpression = dto.IsRecurring ? dto.CronExpression : null;
            foodPlan.UserId = dto.UserId;
            foodPlan.ConsumableType = dto.ConsumableType;

            var jobKey = new JobKey(foodPlan.JobKey);
            var triggerKey = new TriggerKey(foodPlan.TriggerKey);

            var newJobDetail = JobBuilder.Create(dto.ConsumableType == ConsumableType.Food ? typeof(FoodRecordCreationJob) : typeof(RecipeRecordCreationJob))
                .WithIdentity(jobKey)
                .UsingJobData(new JobDataMap
                {
                    { "ObjectId", foodPlan.ConsumableId.ToString() },
                    { "Amount", (float)foodPlan.Amount },
                    { "RequiresConfirmation", foodPlan.RequiresConfirmation },
                    { "ScheduleId", foodPlan.Id },
                    { "ObjectType", foodPlan.ConsumableType }
                })
                .Build();

            await _scheduler.AddJob(newJobDetail, true);

            ITrigger newTrigger = foodPlan.IsRecurring
                ? TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .WithCronSchedule(foodPlan.CronExpression, x => x.InTimeZone(TimeZoneInfo.Utc))
                    .Build()
                : TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartAt(foodPlan.RunAtUtc.Value)
                    .Build();

            await _scheduler.RescheduleJob(triggerKey, newTrigger);
            await _dbContext.SaveChangesAsync();
            return foodPlan;
        }
        public async Task DeleteScheduleAsync(long scheduleId)
        {
            var plan = await _dbContext.FoodPlans.FindAsync(scheduleId);
            if (plan == null || plan.IsDeleted)
                throw new Exception("FoodPlan not found or already deleted");


            var jobKey = new JobKey(plan.JobKey);
            var triggerKey = new TriggerKey(plan.TriggerKey);

            await _scheduler.PauseTrigger(triggerKey);
            await _scheduler.UnscheduleJob(triggerKey);
            await _scheduler.DeleteJob(jobKey);

            plan.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
