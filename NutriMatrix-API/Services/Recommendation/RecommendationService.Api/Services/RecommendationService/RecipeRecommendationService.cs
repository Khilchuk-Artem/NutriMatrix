using Microsoft.EntityFrameworkCore;
using RecommendationService.Api.Models.Dto;
using RecommendationService.Api.Models.Redis;
using RecommendationService.Api.Services.NutrientsAnalysisService;
using Redis.OM;
using Redis.OM.Searching;
using System.Collections.Generic;
using System.Diagnostics;

namespace RecommendationService.Api.Services.RecommendationService
{
    public class RecipeRecommendationService : IRecipeRecommendationService
    {
        private readonly RedisCollection<RecipeShortcutRedis> _recipesCollection;
        private readonly INutrientsAnalysisService _nutrientsAnalysisService;

        public RecipeRecommendationService(RedisCollection<RecipeShortcutRedis> recipesCollection, INutrientsAnalysisService nutrientsAnalysisService)
        {
            _recipesCollection = recipesCollection;
            _nutrientsAnalysisService = nutrientsAnalysisService;
        }

        public async Task<RecommendationResponseDto> GetRecommendationAsync(RecommendationRequestDto dto)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            //step 1: trim down selection by applying filters
            var recipeSpecifications = new List<IQueryable<RecipeShortcutRedis>>();
            foreach(var req in dto.RecipeRequests)
            {
                var tmp = _recipesCollection;
                /*if (req.Category != null) tmp = tmp.Where(r => r.Category == req.Category);
                if (req.IncludeIngredientIds != null) tmp = tmp.Where(r => req.IncludeIngredientIds.All(i=> r.IngredientIds.Contains(i)));
                if (req.ExcludeIngredientIds != null) tmp = tmp.Where(r => req.ExcludeIngredientIds.All(i => !r.IngredientIds.Contains(i)));*/

                recipeSpecifications.Add(tmp.AsQueryable());
            }

            //step 2: define expectations for each of them
            var averages = new List<Dictionary<int, float>>();

            foreach(var req in dto.RecipeRequests)
            {
                averages.Add(await _nutrientsAnalysisService.GetAverageNutrientsPerCategoryAsync(req.Category));
            }

            var expectations = averages
                .Select(r =>
                {
                    var expectations = dto.NutritionalGoals
                        .ToDictionary(
                            nutrient => nutrient.Key,
                            nutrient => nutrient.Value * r[nutrient.Key] / averages.Sum(a => a[nutrient.Key])
                        );

                    return expectations;
                }).ToList();

            //take k nearest neighboors for each candidate
            var k = 200;
            var candidatesList = new List<List<RecipeShortcutRedis>>();

            for(int i = 0; i < recipeSpecifications.Count; i++)
            {
                var rawCandidates =recipeSpecifications[i].ToList();
                rawCandidates = rawCandidates.Where(r => r.NutrientAmounts.Keys.Count!=0).ToList();

                var candidates = rawCandidates
                    .OrderBy(r =>
                        Math.Sqrt(expectations[i].Keys
                            .Sum(k =>
                                (expectations[i][k] - r.NutrientAmounts[k]) * (expectations[i][k] - r.NutrientAmounts[k])/(expectations[i][k]* expectations[i][k])
                            )
                        )
                    )
                    .Take(k)
                    .ToList();


                candidatesList.Add(candidates);
            }

            var bestCombo = FindBestRecipeCombo(float.MaxValue,new Stack<RecipeShortcutRedis>(),new Stack<RecipeShortcutRedis>(),candidatesList, dto.NutritionalGoals);

            sw.Stop();

            return new RecommendationResponseDto()
            {
                RecipesAndAmounts = bestCombo.Item2.Reverse().Select(r => new RecipeWithAmountDto() { Recipe = new RecipeShortcutDto() { Id=r.Id }, Amount= r.Servings }),
                Nutrients = bestCombo.Item2
                .SelectMany(r => r.NutrientAmounts)
                .GroupBy(pair => pair.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(pair => pair.Value)
                ),
                TimeToRespondInMs = sw.ElapsedMilliseconds
        };
        }

        public static Tuple<float, Stack<RecipeShortcutRedis>> FindBestRecipeCombo(
            float bestDistance,
            Stack<RecipeShortcutRedis> bestCombo,
            Stack<RecipeShortcutRedis> currentCombo,
            List<List<RecipeShortcutRedis>> candidatesList,
            Dictionary<int, float> expectations)
        {
            if(currentCombo.Count == candidatesList.Count)
            {

                var distance = currentCombo
                    .Sum(rc =>
                    {
                        return (float)Math.Sqrt(expectations.Keys.Sum(k => (expectations[k] - rc.NutrientAmounts[k]) * (expectations[k] - rc.NutrientAmounts[k])/ (expectations[k] * expectations[k])));
                    });

                return new Tuple<float, Stack<RecipeShortcutRedis>>(distance, new Stack<RecipeShortcutRedis>(currentCombo.Reverse()));
            }

            var routes = new Stack<RecipeShortcutRedis>(candidatesList[currentCombo.Count]);

            foreach(var route in routes)
            {
                currentCombo.Push(route);

                var res = FindBestRecipeCombo(bestDistance, bestCombo,currentCombo,candidatesList,expectations);

                currentCombo.Pop();
                if (res.Item1 < bestDistance)
                {
                    bestDistance = res.Item1;
                    bestCombo = res.Item2;
                }
            }

            return new Tuple<float, Stack<RecipeShortcutRedis>>(bestDistance, new Stack<RecipeShortcutRedis>(bestCombo.Reverse()));
        }

    }
}
