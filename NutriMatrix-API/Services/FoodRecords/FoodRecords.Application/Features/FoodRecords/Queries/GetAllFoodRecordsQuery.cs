using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Specifications.FoodRecords;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodRecords.Queries
{
    public class GetAllFoodRecordsQuery : IRequest<List<FoodRecord>>
    {
        public string UserId { get; set; }
        public bool SortByDateAsc { get; set; } = true;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    public class GetAllFoodRecordsQueryHandler : IRequestHandler<GetAllFoodRecordsQuery, List<FoodRecord>>
    {
        private readonly IRepository<FoodRecord> _repository;

        public GetAllFoodRecordsQueryHandler(IRepository<FoodRecord> repository)
        {
            _repository = repository;
        }

        public async Task<List<FoodRecord>> Handle(GetAllFoodRecordsQuery request, CancellationToken cancellationToken)
        {
            var spec = new FoodRecordsSpecification(request.UserId, request.SortByDateAsc, request.DateFrom, request.DateTo);
            return await _repository.GetAll(spec,1,int.MaxValue);
        }
    }
}
