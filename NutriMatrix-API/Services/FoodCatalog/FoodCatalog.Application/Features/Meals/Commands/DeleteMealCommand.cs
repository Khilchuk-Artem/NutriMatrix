using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Meals.Commands
{
    public class DeleteMealCommand : IRequest<Meal>
    {
        public long Id { get; set; }
    }

    public class DeleteMealCommandHandler : IRequestHandler<DeleteMealCommand, Meal>
    {
        private readonly IRepository<Meal> _mealRepository;

        public DeleteMealCommandHandler(IRepository<Meal> mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public async Task<Meal> Handle(DeleteMealCommand request, CancellationToken cancellationToken)
        {
            var meal = await _mealRepository.Delete(request.Id);
            return meal;
        }
    }
}
