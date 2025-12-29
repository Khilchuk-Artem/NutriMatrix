using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodRecords.Commands
{
    public class DeleteFoodRecordCommand : IRequest<FoodRecord>
    {
        public long Id { get; set; }
    }
    public class DeleteFoodRecordCommandHandler : IRequestHandler<DeleteFoodRecordCommand, FoodRecord>
    {
        private readonly IRepository<FoodRecord> _repository;

        public DeleteFoodRecordCommandHandler(IRepository<FoodRecord> repository)
        {
            _repository = repository;
        }

        public async Task<FoodRecord> Handle(DeleteFoodRecordCommand request, CancellationToken cancellationToken)
        {
            return await _repository.Delete(request.Id);
        }
    }
}
