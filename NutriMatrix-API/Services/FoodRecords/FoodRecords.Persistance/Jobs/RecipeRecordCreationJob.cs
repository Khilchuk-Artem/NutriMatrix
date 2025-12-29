using FoodRecords.Persistance.Data;
using FoodRecords.Persistance.Services.MealFetcher;
using FoodRecords.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace FoodRecords.Persistance.Jobs
{
    public class RecipeRecordCreationJob : IJob
    {
        private readonly FoodRecordsDbContext _dbContext;
        private readonly IMealFetcher _mealFetcher;

        public RecipeRecordCreationJob(FoodRecordsDbContext dbContext, IMealFetcher mealFetcher)
        {
            _dbContext = dbContext;
            _mealFetcher = mealFetcher;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var objectId = (long)context.MergedJobDataMap["ObjectId"];
            var amount = (float)context.MergedJobDataMap["Amount"];
            var requiresConfirmation = (bool)context.MergedJobDataMap["RequiresConfirmation"];
            var scheduleId = (long)context.MergedJobDataMap["ScheduleId"];
            var objectType = (ConsumableType)context.MergedJobDataMap["ObjectType"];


            var job = await _dbContext.FoodPlans.Where(fp => !fp.IsDeleted && fp.Id == scheduleId).FirstOrDefaultAsync();
            if (requiresConfirmation)
            {
                var pendingAddition = new PendingRecord
                {
                    ConsumableType = ConsumableType.Recipe,
                    Amount = amount,
                    UserId = job.UserId,
                    ConsumableId = objectId,
                    DatePending = DateTime.UtcNow,
                    IsDeleted = false
                };

                _dbContext.PendingRecords.Add(pendingAddition);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var dto = await _mealFetcher.FetchMealAsync(objectId);

                var mealRecord = new MealRecord
                {
                    MealId = dto.Id,
                    DateEaten = DateTime.UtcNow,
                    UserId = job.UserId,
                    ServingsEaten = job.Amount,
                    IsDeleted = false,
                    IngredientSnapshots = dto.FoodMeals
                    .Select(fm => new MealIngredientSnapshot
                    {
                        FoodMeasureId = fm.MeasureId,
                        Amount = fm.Quantity * job.Amount / dto.TotalServings
                    })
                    .ToList()
                };

                _dbContext.MealRecords.Add(mealRecord);
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
