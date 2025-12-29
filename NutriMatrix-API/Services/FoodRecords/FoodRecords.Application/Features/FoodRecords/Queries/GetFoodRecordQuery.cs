using FoodRecords.Application.Models.Dto;
using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodRecords.Queries
{
    public class GetFoodRecordQuery : IRequest<FoodRecordDto>
    {
        public long Id { get; set; }
    }
    public class GetFoodRecordQueryHandler : IRequestHandler<GetFoodRecordQuery, FoodRecordDto>
    {
        private readonly IRepository<FoodRecord> _repository;

        public GetFoodRecordQueryHandler(IRepository<FoodRecord> repository)
        {
            _repository = repository;
        }

        public async Task<FoodRecordDto> Handle(GetFoodRecordQuery request, CancellationToken cancellationToken)
        {
            var record = await _repository.Get(request.Id);
            if (record == null) return null;

            return new FoodRecordDto
            {
                RecordId = record.Id,
                FoodMeasureId = record.FoodMeasureId,
                Amount = record.Amount
            };
        }
    }
}
