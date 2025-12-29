using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using MediatR;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodPlans.Commands
{
    public class DeleteFoodPlanCommand : IRequest
    {
        public long Id { get; set; }
    }
    public class DeleteFoodPlanCommandHandler : IRequestHandler<DeleteFoodPlanCommand>
    {
        private readonly IRepository<FoodPlan> _repository;
        private readonly IScheduler _scheduler;

        public DeleteFoodPlanCommandHandler(IRepository<FoodPlan> repository, IScheduler scheduler)
        {
            _repository = repository;
            _scheduler = scheduler;
        }

        public async Task Handle(DeleteFoodPlanCommand request, CancellationToken cancellationToken)
        {
            var plan = await _repository.Get(request.Id);
            if (plan == null || plan.IsDeleted)
                throw new Exception("FoodPlan not found or already deleted");

            var jobKey = new JobKey(plan.JobKey);
            var triggerKey = new TriggerKey(plan.TriggerKey);

            await _scheduler.PauseTrigger(triggerKey);
            await _scheduler.UnscheduleJob(triggerKey);
            await _scheduler.DeleteJob(jobKey);

            await _repository.Delete(request.Id);
        }
    }
}
