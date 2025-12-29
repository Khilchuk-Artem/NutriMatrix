using FoodRecords.Application.Models.Dto;
using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Specifications.MealRecords;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.MealRecords.Commands
{
    public class DeleteMealRecordCommand : IRequest<MealRecordDto>
    {
        public long Id { get; set; }
    }

    public class DeleteMealRecordCommandHandler : IRequestHandler<DeleteMealRecordCommand, MealRecordDto>
    {
        private readonly IRepository<MealRecord> _repository;

        public DeleteMealRecordCommandHandler(IRepository<MealRecord> repository)
        {
            _repository = repository;
        }

        public async Task<MealRecordDto> Handle(DeleteMealRecordCommand request, CancellationToken cancellationToken)
        {
            var mealRecord = await _repository.Delete(request.Id);
            if (mealRecord == null)
                return null;

            return new MealRecordDto
            {
                Id = mealRecord.Id,
                DateEaten = mealRecord.DateEaten,
                UserId = mealRecord.UserId,
                ServingsEaten = mealRecord.ServingsEaten,
            };
        }
    }
}
