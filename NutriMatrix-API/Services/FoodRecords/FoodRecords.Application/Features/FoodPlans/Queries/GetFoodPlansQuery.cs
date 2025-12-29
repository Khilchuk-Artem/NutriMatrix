using FoodRecords.Domain.Contracts;
using FoodRecords.Domain.Entities;
using FoodRecords.Persistance.Specifications.FoodPlans;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Application.Features.FoodPlans.Queries
{
    public class GetFoodPlansQuery : IRequest<List<FoodPlan>>
    {
        public string UserId { get; set; }
        public bool RecurringFirst { get; set; }
        public string SearchByName { get; set; }
    }
    public class GetFoodPlansQueryHandler : IRequestHandler<GetFoodPlansQuery, List<FoodPlan>>
    {
        private readonly IRepository<FoodPlan> _repository;

        public GetFoodPlansQueryHandler(IRepository<FoodPlan> repository)
        {
            _repository = repository;
        }

        public async Task<List<FoodPlan>> Handle(GetFoodPlansQuery request, CancellationToken cancellationToken)
        {
            var spec = new FoodPlansByUserIdSpecification(request.UserId, request.RecurringFirst, request.SearchByName);
            return await _repository.GetAll(spec, 1, int.MaxValue);
        }
    }
}
