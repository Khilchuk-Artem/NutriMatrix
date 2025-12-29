using FoodRecords.Domain.Common;

namespace FoodRecords.Domain.Entities
{
    public class MealIngredientSnapshot:IEntity
    {
        public long Id { get; set; }
        public long FoodMeasureId { get; set; }
        public float Amount { get; set; }
        public bool IsDeleted { get; set; }
    }
}