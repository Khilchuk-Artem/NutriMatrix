namespace Auth.API.Models.DTO
{
    public class UserSummaryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
        public NutrientTracking[] NutrientsToTrack { get; set; }
    }
}
