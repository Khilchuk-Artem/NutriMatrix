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
    public class GetAllMealRecordsQuery : IRequest<List<MealRecordDto>>
    {
        public string UserId { get; set; }
        public bool SortByDateAsc { get; set; } = true;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class GetAllMealRecordsQueryHandler : IRequestHandler<GetAllMealRecordsQuery, List<MealRecordDto>>
    {
        private readonly IRepository<MealRecord> _repository;

        public GetAllMealRecordsQueryHandler(IRepository<MealRecord> repository)
        {
            _repository = repository;
        }

        public async Task<List<MealRecordDto>> Handle(GetAllMealRecordsQuery request, CancellationToken cancellationToken)
        {
            var spec = new MealRecordsByUserSpecification(request.UserId, request.SortByDateAsc, request.DateFrom, request.DateTo);
            var mealRecords = await _repository.GetAll(spec,1,int.MaxValue);

            return mealRecords.Select(mr => new MealRecordDto
            {
                MealId = mr.MealId,
                Id = mr.Id,
                DateEaten = mr.DateEaten,
                UserId = mr.UserId,
                ServingsEaten = mr.ServingsEaten,
                IngredientSnapshots = mr.IngredientSnapshots
                    .Select(s => new MealIngredientSnapshotDto
                    {
                        FoodMeasureId = s.FoodMeasureId,
                        Amount = s.Amount
                    })
                    .ToList()
            }).ToList();
        }
    }
}
