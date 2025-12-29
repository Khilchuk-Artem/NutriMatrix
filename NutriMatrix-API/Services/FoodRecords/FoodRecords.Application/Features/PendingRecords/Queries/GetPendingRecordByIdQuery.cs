using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.PendingRecords.Queries
{
    public class GetPendingRecordByIdQuery : IRequest<PendingRecord>
    {
        public long Id { get; set; }
    }
    public class GetPendingRecordByIdQueryHandler : IRequestHandler<GetPendingRecordByIdQuery, PendingRecord>
    {
        private readonly IRepository<PendingRecord> _repository;

        public GetPendingRecordByIdQueryHandler(IRepository<PendingRecord> repository)
        {
            _repository = repository;
        }

        public async Task<PendingRecord> Handle(GetPendingRecordByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.Get(request.Id);
        }
    }
}
