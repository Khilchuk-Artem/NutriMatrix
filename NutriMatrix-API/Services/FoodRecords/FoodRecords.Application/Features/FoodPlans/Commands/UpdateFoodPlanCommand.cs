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
    public class UpdateFoodPlanCommand : IRequest<FoodPlan>
    {
        public long Id { get; set; }
        public ScheduleDto Dto { get; set; }
    }
    public class UpdateFoodPlanCommandHandler : IRequestHandler<UpdateFoodPlanCommand, FoodPlan>
    {
        private readonly IRepository<FoodPlan> _repository;
        private readonly IScheduler _scheduler;

        public UpdateFoodPlanCommandHandler(IRepository<FoodPlan> repository, IScheduler scheduler)
        {
            _repository = repository;
            _scheduler = scheduler;
        }

        public async Task<FoodPlan> Handle(UpdateFoodPlanCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (dto.Id == null || !Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                throw new ArgumentException("Invalid Id or ConsumableType");
            if (string.IsNullOrEmpty(dto.UserId))
                throw new ArgumentException("UserId cannot be null or empty");
            if (dto.Id != request.Id)
                throw new ArgumentException("ID mismatch");

            var foodPlan = await _repository.Get(request.Id);
            if (foodPlan == null || foodPlan.IsDeleted)
                throw new Exception("FoodPlan not found or deleted");

            foodPlan.ConsumableId = dto.ConsumableId;
            foodPlan.Amount = dto.Amount;
            foodPlan.RequiresConfirmation = dto.RequiresConfirmation;
            foodPlan.IsRecurring = dto.IsRecurring;
            foodPlan.RunAtUtc = dto.IsRecurring ? null : dto.RunAtUtc;
            foodPlan.CronExpression = dto.IsRecurring ? dto.CronExpression : null;
            foodPlan.UserId = dto.UserId;
            foodPlan.ConsumableType = dto.ConsumableType;
            foodPlan.Name = dto.Name;

            var jobKey = new JobKey(foodPlan.JobKey);
            var triggerKey = new TriggerKey(foodPlan.TriggerKey);

            var newJobDetail = JobBuilder.Create(dto.ConsumableType == ConsumableType.Food ? typeof(FoodRecordCreationJob) : typeof(RecipeRecordCreationJob))
                .WithIdentity(jobKey)
                .UsingJobData(new JobDataMap
                {
                { "ObjectId", foodPlan.ConsumableId.ToString() },
                { "Amount", foodPlan.Amount },
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
            await _repository.Update(foodPlan);
            return foodPlan;
        }
    }
}
