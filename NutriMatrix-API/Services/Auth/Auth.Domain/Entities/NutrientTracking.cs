using Auth.Domain.Common;

namespace Auth.Domain.Entities
{
    public class NutrientTracking:IEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long NutrientId { get; set; }
        public float TargetAmount { get; set; }
        public bool IsActive { get; set; }
    }
}
