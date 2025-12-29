using FoodRecords.Application.Dto;
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
    public class UpdatePendingRecordCommand : IRequest
    {
        public long Id { get; set; }
        public PendingAdditionDto Dto { get; set; }
    }
    public class UpdatePendingRecordCommandHandler : IRequestHandler<UpdatePendingRecordCommand>
    {
        private readonly IRepository<PendingRecord> _repository;

        public UpdatePendingRecordCommandHandler(IRepository<PendingRecord> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdatePendingRecordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (!Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                throw new ArgumentException("Invalid ConsumableType");

            var pendingRecord = await _repository.Get(request.Id);
            if (pendingRecord == null)
                throw new Exception("PendingRecord not found");

            pendingRecord.ConsumableType = dto.ConsumableType;
            pendingRecord.Amount = dto.Amount;
            pendingRecord.UserId = dto.UserId;
            pendingRecord.DatePending = dto.DatePending;
            pendingRecord.ConsumableId = dto.ConsumableId;

            await _repository.Update(pendingRecord);
        }
    }
}
