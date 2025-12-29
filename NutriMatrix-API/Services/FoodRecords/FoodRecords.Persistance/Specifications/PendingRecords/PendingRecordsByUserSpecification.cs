using Ardalis.Specification;
using FoodRecords.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Persistance.Specifications.PendingRecords
{
    public class PendingRecordsByUserSpecification : Specification<PendingRecord>
    {
        public PendingRecordsByUserSpecification(string userId, DateTime? startDate, DateTime? endDate)
        {
            Query.Where(pa => pa.UserId == userId && !pa.IsDeleted);

            if (startDate.HasValue)
                Query.Where(pa => pa.DatePending >= startDate.Value);

            if (endDate.HasValue)
                Query.Where(pa => pa.DatePending <= endDate.Value);
        }
    }
}
