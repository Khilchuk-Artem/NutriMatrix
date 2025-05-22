namespace Auth.API.Models
{
    public class NutrientTracking
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long NutrientId { get; set; }
        public float TargetAmount { get; set; }
        public bool IsActive { get; set; }
    }
}
