namespace RecommendationService.Api.Models.Dto
{
    public class RecipeShortcutDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public float TotalServings { get; set; }
    }
}
