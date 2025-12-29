using FoodRecords.Application.Models.Dto;
using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.MealRecords.Commands
{
    public class UpdateMealRecordCommand : IRequest<MealRecordDto>
    {
        public long Id { get; set; }
        public UpdateMealRecordDto Dto { get; set; }
    }

    public class UpdateMealRecordCommandHandler : IRequestHandler<UpdateMealRecordCommand, MealRecordDto>
    {
        private readonly IRepository<MealRecord> _repository;
        private readonly FoodRecordsDbContext _dbContext;

        public UpdateMealRecordCommandHandler(IRepository<MealRecord> repository, FoodRecordsDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public async Task<MealRecordDto> Handle(UpdateMealRecordCommand request, CancellationToken cancellationToken)
        {
            var mealRecord = await _dbContext.MealRecords
                .Include(mr => mr.IngredientSnapshots)
                .FirstOrDefaultAsync(mr => mr.Id == request.Id && !mr.IsDeleted);

            if (mealRecord == null)
            {
                return null;
            }

            mealRecord.DateEaten = request.Dto.DateEaten;
            mealRecord.ServingsEaten = request.Dto.ServingsEaten;
            mealRecord.MealId = request.Dto.MealId;

            _dbContext.RemoveRange(mealRecord.IngredientSnapshots);

            mealRecord.IngredientSnapshots = request.Dto.IngredientSnapshots
                .Select(s => new MealIngredientSnapshot
                {
                    FoodMeasureId = s.FoodMeasureId,
                    Amount = s.Amount,
                    IsDeleted = false
                })
                .ToList();

            await _dbContext.SaveChangesAsync();

            var mealRecordDto = new MealRecordDto
            {
                MealId = mealRecord.MealId,
                Id = mealRecord.Id,
                DateEaten = mealRecord.DateEaten,
                UserId = mealRecord.UserId,
                ServingsEaten = mealRecord.ServingsEaten,
                IngredientSnapshots = mealRecord.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshotDto
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            };

            return mealRecordDto;
        }
    }
}
