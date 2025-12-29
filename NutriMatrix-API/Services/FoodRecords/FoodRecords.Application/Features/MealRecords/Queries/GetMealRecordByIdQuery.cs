using FoodRecords.Application.Models.Dto;
using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Specifications.MealRecords;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.MealRecords.Queries
{
    public class GetMealRecordByIdQuery : IRequest<MealRecordDto>
    {
        public long Id { get; set; }
    }

    public class GetMealRecordByIdQueryHandler : IRequestHandler<GetMealRecordByIdQuery, MealRecordDto>
    {
        private readonly IRepository<MealRecord> _repository;

        public GetMealRecordByIdQueryHandler(IRepository<MealRecord> repository)
        {
            _repository = repository;
        }

        public async Task<MealRecordDto> Handle(GetMealRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var includeIngredientsSpec = new MealRecordWithIngredientSpec();
            var mealRecord = await _repository.Get(request.Id, includeIngredientsSpec);
            if (mealRecord == null)
                return null;

            return new MealRecordDto
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
                        Amount = s.Amount,
                        Id = s.Id
                    })
                    .ToList()
            };
        }
    }
}
