using Ardalis.Specification;
using Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Auth.Persistance.Specifications
{
    public class NutrientTrackingsByUserIdSpecification : Specification<NutrientTracking>
    {
        public NutrientTrackingsByUserIdSpecification(string userId)
        {
            Query.Where(nt => nt.UserId == userId);
        }
    }
}
