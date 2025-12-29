using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodPlans.Queries
{
    public class GetFoodPlanByIdQuery : IRequest<FoodPlan>
    {
        public long Id { get; set; }
    }
    public class GetFoodPlanByIdQueryHandler : IRequestHandler<GetFoodPlanByIdQuery, FoodPlan>
    {
        private readonly IRepository<FoodPlan> _repository;

        public GetFoodPlanByIdQueryHandler(IRepository<FoodPlan> repository)
        {
            _repository = repository;
        }

        public async Task<FoodPlan> Handle(GetFoodPlanByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.Get(request.Id);
        }
    }
}
