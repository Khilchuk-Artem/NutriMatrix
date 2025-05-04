
using RecommendationService.Api.Models.Redis;
using Redis.OM.Searching;
using System.Text.Json;

namespace RecommendationService.Api.Services.NutrientsAnalysisService
{
    public class NutrientsAnalysisService : INutrientsAnalysisService
    {
        private readonly RedisCollection<RecipeShortcutRedis> _recipes;
        private readonly RedisCollection<CategoryAverageNutrientsRedis> _computedAverages;
        public NutrientsAnalysisService(RedisCollection<RecipeShortcutRedis> recipes, RedisCollection<CategoryAverageNutrientsRedis> computedAverages)
        {
            _recipes = recipes;
            _computedAverages = computedAverages;
            
        }
        public async Task<Dictionary<int, float>> GetAverageNutrientsPerCategoryAsync(string category)
        {
            var averages = _computedAverages
                .Where(ca => ca.Category == category)
                .FirstOrDefault();

            if (averages != null) return averages.Amounts;

            var computedAverage = _recipes
                .Where(r => r.Category == category)
                .ToList()
                .SelectMany(r => r.NutrientAmounts)
                .GroupBy(pair => pair.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(pair => pair.Value)
                );

            var newComputedAverage = new CategoryAverageNutrientsRedis()
            {
                Category = category,
                AmountsJson = JsonSerializer.Serialize(computedAverage)
            };

            await _computedAverages.InsertAsync(newComputedAverage);

            return computedAverage;
        }

        public async Task RefreshComputedAverages()
        {
            var recipeData = await _recipes.ToListAsync();

            var categoryAverages = recipeData
                .GroupBy(r => r.Category)
                .Select(g =>
                {
                    var averageNutrients = g
                        .SelectMany(r => r.NutrientAmounts)
                        .GroupBy(pair => pair.Key)
                        .ToDictionary(
                            ng => ng.Key,
                            ng => ng.Average(p => p.Value)
                        );

                    var amountsJson = JsonSerializer.Serialize(averageNutrients);

                    return new CategoryAverageNutrientsRedis()
                    {
                        Category = g.Key,
                        AmountsJson = amountsJson
                    };
                })
                .ToList();

            foreach (var catAvg in categoryAverages)
            {
                await _computedAverages.InsertAsync(catAvg);
            }
        }

    }
}
