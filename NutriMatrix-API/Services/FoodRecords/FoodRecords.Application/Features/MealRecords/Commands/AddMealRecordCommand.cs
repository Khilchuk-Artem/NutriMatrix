using FoodRecords.Application.Models.Dto;
using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.MealRecords.Commands
{
    public class AddMealRecordCommand : IRequest<MealRecordDto>
    {
        public AddMealRecordDto Dto { get; set; }
    }

    public class AddMealRecordCommandHandler : IRequestHandler<AddMealRecordCommand, MealRecordDto>
    {
        private readonly IRepository<MealRecord> _repository;

        public AddMealRecordCommandHandler(IRepository<MealRecord> repository)
        {
            _repository = repository;
        }

        public async Task<MealRecordDto> Handle(AddMealRecordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var mealRecord = new MealRecord
            {
                MealId = dto.MealId,
                DateEaten = dto.DateEaten,
                UserId = dto.UserId,
                ServingsEaten = dto.ServingsEaten,
                IsDeleted = false,
                IngredientSnapshots = dto.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshot
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            };

            var addedRecord = await _repository.Add(mealRecord);

            return new MealRecordDto
            {
                MealId = addedRecord.MealId,
                Id = addedRecord.Id,
                DateEaten = addedRecord.DateEaten,
                UserId = addedRecord.UserId,
                ServingsEaten = addedRecord.ServingsEaten,
                IngredientSnapshots = addedRecord.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshotDto
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            };
        }
    }
}
