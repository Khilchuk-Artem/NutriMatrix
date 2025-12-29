using FoodRecords.Application.Models.Dto;
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
    public class AddFoodRecordCommand : IRequest<FoodRecord>
    {
        public AddFoodRecordDto Dto { get; set; }
    }
    public class AddFoodRecordCommandHandler : IRequestHandler<AddFoodRecordCommand, FoodRecord>
    {
        private readonly IRepository<FoodRecord> _repository;

        public AddFoodRecordCommandHandler(IRepository<FoodRecord> repository)
        {
            _repository = repository;
        }

        public async Task<FoodRecord> Handle(AddFoodRecordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var newRecord = new FoodRecord
            {
                DateEaten = dto.DateEaten,
                UserId = dto.UserId,
                FoodMeasureId = dto.FoodMeasureId,
                Amount = dto.Amount,
                IsDeleted = false
            };
            return await _repository.Add(newRecord);
        }
    }

}
