using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Services.MealFetcher;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.PendingRecords.Commands
{
    public class ConfirmPendingRecordCommand : IRequest
    {
        public long Id { get; set; }
    }
    public class ConfirmPendingRecordCommandHandler : IRequestHandler<ConfirmPendingRecordCommand>
    {
        private readonly IRepository<PendingRecord> _pendingRepository;
        private readonly IRepository<FoodRecord> _foodRepository;
        private readonly IRepository<MealRecord> _mealRepository;
        private readonly IMealFetcher _mealFetcher;

        public ConfirmPendingRecordCommandHandler(
            IRepository<PendingRecord> pendingRepository,
            IRepository<FoodRecord> foodRepository,
            IRepository<MealRecord> mealRepository,
            IMealFetcher mealFetcher)
        {
            _pendingRepository = pendingRepository;
            _foodRepository = foodRepository;
            _mealRepository = mealRepository;
            _mealFetcher = mealFetcher;
        }

        public async Task Handle(ConfirmPendingRecordCommand request, CancellationToken cancellationToken)
        {
            var pendingRecord = await _pendingRepository.Get(request.Id);
            if (pendingRecord == null)
                throw new Exception("PendingRecord not found");

            pendingRecord.IsDeleted = true;
            await _pendingRepository.Update(pendingRecord);

            if (pendingRecord.ConsumableType == ConsumableType.Food)
            {
                var foodRecord = new FoodRecord
                {
                    DateEaten = pendingRecord.DatePending,
                    UserId = pendingRecord.UserId,
                    FoodMeasureId = pendingRecord.ConsumableId,
                    Amount = pendingRecord.Amount
                };
                await _foodRepository.Add(foodRecord);
            }
            else
            {
                var dto = await _mealFetcher.FetchMealAsync(pendingRecord.ConsumableId);
                var mealRecord = new MealRecord
                {
                    MealId = dto.Id,
                    DateEaten = pendingRecord.DatePending,
                    UserId = pendingRecord.UserId,
                    ServingsEaten = pendingRecord.Amount,
                    IsDeleted = false,
                    IngredientSnapshots = dto.FoodMeals
                        .Select(fm => new MealIngredientSnapshot
                        {
                            FoodMeasureId = fm.MeasureId,
                            Amount = fm.Quantity * pendingRecord.Amount / dto.TotalServings
                        })
                        .ToList()
                };
                await _mealRepository.Add(mealRecord);
            }
        }
    }
}
