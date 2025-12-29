using Ardalis.Specification;
using FoodRecords.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Persistance.Specifications.MealRecords
{
    public class MealRecordWithIngredientSpec:Specification<MealRecord>
    {
        public MealRecordWithIngredientSpec()
        {
            Query.Where(r =>!r.IsDeleted).Include(mr => mr.IngredientSnapshots);
        }
    }
}
