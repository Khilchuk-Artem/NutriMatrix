using FoodRecords.Api.Data;
using FoodRecords.Api.Models.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;

namespace FoodRecords.Api.Jobs
{
    public class FoodRecordCreationJob : IJob
    {
        private readonly FoodRecordsDbContext _dbContext;

        public FoodRecordCreationJob(FoodRecordsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var objectId = (long)context.MergedJobDataMap["ObjectId"];
            var amount = (float)context.MergedJobDataMap["Amount"];
            var requiresConfirmation = (bool)context.MergedJobDataMap["RequiresConfirmation"];
            var scheduleId = (long)context.MergedJobDataMap["ScheduleId"];
            var objectType = (ConsumableType)context.MergedJobDataMap["ObjectType"];


            var job =await _dbContext.FoodPlans.Where(fp => !fp.IsDeleted && fp.Id == scheduleId).FirstOrDefaultAsync();
            if (requiresConfirmation)
            {
                var pendingAddition = new PendingRecord
                {
                    ConsumableType = ConsumableType.Food,
                    Amount = amount,
                    ConsumableId = objectId,
                    UserId = job.UserId,
                    DatePending = DateTime.UtcNow,
                    IsDeleted = false
                };

                _dbContext.PendingRecords.Add(pendingAddition);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var foodRecord = new FoodRecord()
                {
                    DateEaten = DateTime.UtcNow,
                    UserId = job.UserId,
                    FoodMeasureId = objectId,
                    Amount = amount,
                    IsDeleted = false
                };

                _dbContext.FoodRecords.Add(foodRecord);
                await _dbContext.SaveChangesAsync();
            }

            if (!job.IsRecurring)
            {
                job.IsDeleted = true;
                await context.Scheduler.DeleteJob(context.JobDetail.Key);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
