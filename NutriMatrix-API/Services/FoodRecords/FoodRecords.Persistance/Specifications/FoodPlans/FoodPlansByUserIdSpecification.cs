using Ardalis.Specification;
using FoodRecords.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Persistance.Specifications.FoodPlans
{
    public class FoodPlansByUserIdSpecification : Specification<FoodPlan>
    {
        public FoodPlansByUserIdSpecification(string userId, bool recurringFirst, string searchByName)
        {
            Query.Where(fp => !fp.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchByName))
                Query.Where(fp => fp.Name.Contains(searchByName));

            if (!string.IsNullOrWhiteSpace(userId))
                Query.Where(fp => fp.UserId == userId);

            if (recurringFirst)
                Query.OrderByDescending(fp => fp.IsRecurring);
        }
    }
}
