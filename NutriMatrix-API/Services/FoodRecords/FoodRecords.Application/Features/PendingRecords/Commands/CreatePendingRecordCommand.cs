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
    public class CreatePendingRecordCommand : IRequest<PendingRecord>
    {
        public PendingAdditionDto Dto { get; set; }
    }
    public class CreatePendingRecordCommandHandler : IRequestHandler<CreatePendingRecordCommand, PendingRecord>
    {
        private readonly IRepository<PendingRecord> _repository;

        public CreatePendingRecordCommandHandler(IRepository<PendingRecord> repository)
        {
            _repository = repository;
        }

        public async Task<PendingRecord> Handle(CreatePendingRecordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (!Enum.IsDefined(typeof(ConsumableType), dto.ConsumableType))
                throw new ArgumentException("Invalid ConsumableType");

            var pendingRecord = new PendingRecord
            {
                ConsumableType = dto.ConsumableType,
                Amount = dto.Amount,
                UserId = dto.UserId,
                ConsumableId = dto.ConsumableId,
                DatePending = dto.DatePending,
                IsDeleted = false
            };

            return await _repository.Add(pendingRecord);
        }
    }
}
