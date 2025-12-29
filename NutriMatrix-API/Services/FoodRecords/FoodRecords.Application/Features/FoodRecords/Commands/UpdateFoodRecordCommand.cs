using FoodRecords.Domain.Contracts;
using FoodRecords.Application.Models.Dto;
using FoodRecords.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodRecords.Commands
{
    public class UpdateFoodRecordCommand : IRequest<FoodRecord>
    {
        public long Id { get; set; }
        public UpdateFoodRecordDto Dto { get; set; }
    }
    public class UpdateFoodRecordCommandHandler : IRequestHandler<UpdateFoodRecordCommand, FoodRecord>
    {
        private readonly IRepository<FoodRecord> _repository;

        public UpdateFoodRecordCommandHandler(IRepository<FoodRecord> repository)
        {
            _repository = repository;
        }

        public async Task<FoodRecord> Handle(UpdateFoodRecordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var entity = await _repository.Get(request.Id);
            if (entity == null) return null;

            entity.Amount = dto.Amount;
            entity.FoodMeasureId = dto.FoodMeasureId;
            return await _repository.Update(entity);
        }
    }
}
