using Ardalis.Specification;
using FoodRecords.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Persistance.Specifications.FoodRecords
{
    public class FoodRecordsSpecification : Specification<FoodRecord>
    {
        public FoodRecordsSpecification(string userId, bool sortByDateAsc, DateTime? dateFrom, DateTime? dateTo)
        {
            Query.Where(r => r.UserId == userId && !r.IsDeleted);

            if (dateFrom.HasValue)
                Query.Where(r => r.DateEaten >= dateFrom.Value);

            if (dateTo.HasValue)
                Query.Where(r => r.DateEaten <= dateTo.Value);

            if (sortByDateAsc)
                Query.OrderBy(r => r.DateEaten);
            else
                Query.OrderByDescending(r => r.DateEaten);
        }
    }
}
