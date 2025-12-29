using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.PendingRecords.Commands
{
    public class DeletePendingRecordCommand : IRequest
    {
        public long Id { get; set; }
    }
    public class DeletePendingRecordCommandHandler : IRequestHandler<DeletePendingRecordCommand>
    {
        private readonly IRepository<PendingRecord> _repository;

        public DeletePendingRecordCommandHandler(IRepository<PendingRecord> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeletePendingRecordCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.Delete(request.Id);
            if (entity == null)
                throw new Exception("PendingRecord not found");
        }
    }
}
