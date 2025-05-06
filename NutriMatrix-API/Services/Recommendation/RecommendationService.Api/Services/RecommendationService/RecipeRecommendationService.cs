using Microsoft.EntityFrameworkCore;
using Qdrant.Client.Grpc;
using RecommendationService.Api.Models.Dto;
using RecommendationService.Api.Models.Redis;
using RecommendationService.Api.Services.NutrientsAnalysisService;
using RecommendationService.Api.Services.Qdrant;
using Redis.OM;
using Redis.OM.Searching;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RecommendationService.Api.Services.RecommendationService
{
    public class RecipeRecommendationService : IRecipeRecommendationService
    {
        private readonly RedisCollection<RecipeShortcutRedis> _recipesCollection;
        private readonly INutrientsAnalysisService _nutrientsAnalysisService;
        private readonly IQdrantService _qdrant;
        public RecipeRecommendationService(RedisCollection<RecipeShortcutRedis> recipesCollection, INutrientsAnalysisService nutrientsAnalysisService, IQdrantService qdrant)
        {
            _recipesCollection = recipesCollection;
            _nutrientsAnalysisService = nutrientsAnalysisService;
            _qdrant = qdrant;
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
                var reference = _recipesCollection.Where(r=>r.Id== 46989).ToList();

                var vector = reference[0].NutrientAmounts.Keys
                    .Select(k => expectations[i].TryGetValue(k, out var value) ? value : 0f)
                    .ToList();
                var pastIds = candidatesList.SelectMany(innerList => innerList).Select(r=>(int)r.Id);

                var candidateIds = await _qdrant.FindKNearestNeighborsAsync(300, vector, pastIds);


                var candidates = new List<RecipeShortcutRedis>();

                foreach (var a in candidateIds)
                {
                    var candidate = await _recipesCollection.FirstAsync(r => r.Id == a);

                    candidates.Add(candidate);
                }

                candidatesList.Add(candidates);
            }

            var bestCombo = FindBestRecipeCombo(float.MaxValue, new Stack<Tuple<RecipeShortcutRedis, int>>(), new Stack<Tuple<RecipeShortcutRedis, int>>(), candidatesList, dto.NutritionalGoals);
            sw.Stop();


            return new RecommendationResponseDto()
            {
                RecipesAndAmounts = bestCombo.Item2.Reverse().Select(r => new RecipeWithAmountDto() { Recipe = new RecipeShortcutDto() { Id=r.Item1.Id }, Amount= r.Item2 }),
                Nutrients = bestCombo.Item2
                .SelectMany(r => r.Item1.NutrientAmounts.ToDictionary(rr=>rr.Key,rr=>rr.Value*r.Item2/r.Item1.Servings))
                .GroupBy(pair => pair.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(pair => pair.Value)
                ),
                TimeToRespondInMs = sw.ElapsedMilliseconds
        };
        }

        public static Tuple<float, Stack<Tuple<RecipeShortcutRedis,int>>> FindBestRecipeCombo(
            float bestDistance,
            Stack<Tuple<RecipeShortcutRedis, int>> bestCombo,
            Stack<Tuple<RecipeShortcutRedis, int>> currentCombo,
            List<List<RecipeShortcutRedis>> candidatesList,
            Dictionary<int, float> expectations)
        {
            var currentDistance = (float)Math.Sqrt(expectations.Keys
                    .Sum(k =>
                    {
                        return (expectations[k] - currentCombo.Sum(rc => rc.Item1.NutrientAmounts[k] * rc.Item2 / rc.Item1.Servings)) * (expectations[k] - currentCombo.Sum(rc => rc.Item1.NutrientAmounts[k] * rc.Item2 / rc.Item1.Servings));
                    }));
            if (currentCombo.Count == candidatesList.Count|| currentDistance>bestDistance)
            {
                return new Tuple<float, Stack<Tuple<RecipeShortcutRedis, int>>>(currentDistance, new Stack<Tuple<RecipeShortcutRedis, int>>(currentCombo.Reverse()));
            }

            var routes = new Stack<RecipeShortcutRedis>(candidatesList[currentCombo.Count]);

            foreach(var route in routes)
            {
                for(int amount = 1; amount < route.Servings; amount++)
                {
                    currentCombo.Push(new Tuple<RecipeShortcutRedis, int>(route,amount));

                    var res = FindBestRecipeCombo(bestDistance, bestCombo, currentCombo, candidatesList, expectations);

                    currentCombo.Pop();
                    if (res.Item1 < bestDistance)
                    {
                        bestDistance = res.Item1;
                        bestCombo = res.Item2;
                    }
                }
            }

            return new Tuple<float, Stack<Tuple<RecipeShortcutRedis, int>>>(bestDistance, new Stack<Tuple<RecipeShortcutRedis, int>>(bestCombo.Reverse()));
        }

    }
}
