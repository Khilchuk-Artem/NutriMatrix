using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Specifications.PendingRecords;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.PendingRecords.Queries
{
    public class GetPendingRecordsQuery : IRequest<List<PendingRecord>>
    {
        public string UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class GetPendingRecordsQueryHandler : IRequestHandler<GetPendingRecordsQuery, List<PendingRecord>>
    {
        private readonly IRepository<PendingRecord> _repository;

        public GetPendingRecordsQueryHandler(IRepository<PendingRecord> repository)
        {
            _repository = repository;
        }

        public async Task<List<PendingRecord>> Handle(GetPendingRecordsQuery request, CancellationToken cancellationToken)
        {
            var spec = new PendingRecordsByUserSpecification(request.UserId, request.StartDate, request.EndDate);
            return await _repository.GetAll(spec, 1, int.MaxValue);
        }
    }

    
}
