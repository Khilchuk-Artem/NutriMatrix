using FoodRecords.Application.Services.TaskSchedulerService;
using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Jobs;
using MediatR;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodPlans.Commands
{
    public class CreateFoodPlanCommand : IRequest<FoodPlan>
    {
        public ScheduleDto Dto { get; set; }
    }
    public class CreateFoodPlanCommandHandler : IRequestHandler<CreateFoodPlanCommand, FoodPlan>
    {
        private readonly IRepository<FoodPlan> _repository;
        private readonly IScheduler _scheduler;

        public CreateFoodPlanCommandHandler(IRepository<FoodPlan> repository, IScheduler scheduler)
        {
            _repository = repository;
            _scheduler = scheduler;
        }

        public async Task<FoodPlan> Handle(CreateFoodPlanCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (!Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                throw new ArgumentException("ConsumableType must be 'Food' or 'Recipe'");
            if (string.IsNullOrEmpty(dto.UserId))
                throw new ArgumentException("UserId cannot be null or empty");

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

            foodPlan = await _repository.Add(foodPlan);

            var finalJobKeyStr = $"objectJob-{foodPlan.Id}";
            var finalTriggerKeyStr = $"trigger-{foodPlan.Id}";
            foodPlan.JobKey = finalJobKeyStr;
            foodPlan.TriggerKey = finalTriggerKeyStr;
            await _repository.Update(foodPlan);

            var jobKey = new JobKey(finalJobKeyStr);
            var triggerKey = new TriggerKey(finalTriggerKeyStr);

            var jobDetail = JobBuilder.Create(dto.ConsumableType == ConsumableType.Food ? typeof(FoodRecordCreationJob) : typeof(RecipeRecordCreationJob))
                .WithIdentity(jobKey)
                .UsingJobData(new JobDataMap
                {
                { "ObjectId", foodPlan.ConsumableId },
                { "Amount", foodPlan.Amount },
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
    }
}
