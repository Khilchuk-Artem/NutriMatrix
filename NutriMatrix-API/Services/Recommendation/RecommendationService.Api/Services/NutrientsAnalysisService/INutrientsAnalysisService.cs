namespace RecommendationService.Api.Services.NutrientsAnalysisService
{
    public interface INutrientsAnalysisService
    {
        public Task<Dictionary<int, float>> GetAverageNutrientsPerCategoryAsync(string Category);
    }
}
