using Microsoft.EntityFrameworkCore;
using Qdrant.Client.Grpc;
using RecommendationService.Api.Data;
using RecommendationService.Api.Models;
using RecommendationService.Api.Models.Dto;
using RecommendationService.Api.Models.Redis;
using RecommendationService.Api.Services.NutrientsAnalysisService;
using RecommendationService.Api.Services.Qdrant;
using Redis.OM;
using Redis.OM.Searching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RecommendationService.Api.Services.RecommendationService
{
    public partial class RecipeRecommendationService : IRecipeRecommendationService
    {
        private readonly RedisCollection<RecipeShortcutRedis> _recipesCollection;
        private readonly IQdrantService _qdrant;
        private readonly RecipeDbContext _db;

        private static readonly TimeSpan _timeLimit = TimeSpan.FromSeconds(7);
        private Stopwatch _searchStopwatch;

        private readonly Random _rng = new Random();

        public RecipeRecommendationService(
            RedisCollection<RecipeShortcutRedis> recipesCollection,
            IQdrantService qdrant,
            RecipeDbContext db)
        {
            _recipesCollection = recipesCollection;
            _qdrant = qdrant;
            _db = db;
        }

        public async Task<RecommendationResponseDto> GetRecommendationAsync(RecommendationRequestDto dto)
        {
            var sw = Stopwatch.StartNew();
            _searchStopwatch = Stopwatch.StartNew();

            var reference = await _recipesCollection
                .Where(r => r.Id == 46989)
                .ToListAsync();
            var vector = reference[0].NutrientAmounts.Keys
                .Select(k => dto.NutritionalGoals.TryGetValue(k, out var v) ? v : 0f)
                .ToList();
            var candidatesList = new List<List<RecipeShortcutRedis>>();
            for (int i = 0; i < dto.RecipeRequests.Count(); i++)
            {
                var pastIds = candidatesList.SelectMany(l => l).Select(r => (int)r.Id);
                var candidateIds = await _qdrant
                    .FindKNearestNeighborsAsync(300, vector, pastIds, dto.RecipeRequests[i].Category, dto.RecipeRequests[i].IncludeIngredientIds);

                var candidates = new List<RecipeShortcutRedis>();
                foreach (var id in candidateIds)
                {
                    var item = await _recipesCollection.FirstOrDefaultAsync(r => r.Id == id);
                    if (item == null)
                    {
                        var fromDb = await _db.Recipes
                            .Include(r => r.Measures)
                            .Include(r => r.NutrientsPerTotalServings)
                            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
                        if (fromDb != null)
                        {
                            item = new RecipeShortcutRedis
                            {
                                Id = fromDb.Id,
                                RecipeId = fromDb.Id,
                                Title = fromDb.Title,
                                Category = fromDb.Category,
                                Servings = fromDb.Servings ?? 0,
                                IngredientIds = fromDb.Measures
                                    .Where(m => !m.IsDeleted)
                                    .Select(m => m.FoodId)
                                    .Distinct()
                                    .ToList(),
                                NutrientAmounts = fromDb.NutrientsPerTotalServings
                                    .Where(n => !n.IsDeleted)
                                    .ToDictionary(n => n.NutrientId, n => n.Amount)
                            };
                        }
                    }
                    if (item != null)
                        candidates.Add(item);
                }
                candidatesList.Add(candidates);
            }

            int populationSize = 50;
            int numSlots = dto.RecipeRequests.Count();
            var population = new List<Individual>(populationSize);
            Individual bestIndividual = null;

            for (int i = 0; i < populationSize; i++)
            {
                var indiv = CreateRandomIndividual(candidatesList, numSlots);
                indiv.Fitness = EvaluateFitness(indiv, dto.NutritionalGoals);
                population.Add(indiv);
                if (bestIndividual == null || indiv.Fitness < bestIndividual.Fitness)
                    bestIndividual = indiv;
            }

            while (_searchStopwatch.Elapsed < _timeLimit)
            {
                var newPopulation = new List<Individual>(populationSize);
                for (int i = 0; i < populationSize; i++)
                {
                    var parent1 = TournamentSelect(population, 3);
                    var parent2 = TournamentSelect(population, 3);

                    var child = Crossover(parent1, parent2, numSlots);

                    Mutate(child, candidatesList);
                    child.Fitness = EvaluateFitness(child, dto.NutritionalGoals);

                    newPopulation.Add(child);
                    if (child.Fitness < bestIndividual.Fitness)
                        bestIndividual = child;

                    if (_searchStopwatch.Elapsed >= _timeLimit)
                        break;
                }

                population = newPopulation;
            }

            var bestCombo = bestIndividual.Genes
                .Select(g => (g.Recipe, g.Amount))
                .Reverse()
                .ToList();
            sw.Stop();

            var totalDistance = (float)Math.Sqrt(dto.NutritionalGoals.Keys
                .Sum(k =>
                {
                    float actual = bestCombo
                        .Sum(rc => rc.Recipe.NutrientAmounts.GetValueOrDefault(k, 0f) * rc.Amount / rc.Recipe.Servings);
                    var diff = dto.NutritionalGoals[k] - actual;
                    return diff * diff / (dto.NutritionalGoals[k] * dto.NutritionalGoals[k]);
                }));

            var aggregated = bestCombo
                .SelectMany(rc => rc.Recipe.NutrientAmounts
                    .Where(n => dto.NutritionalGoals.ContainsKey(n.Key))
                    .ToDictionary(n => n.Key, n => n.Value * rc.Amount / rc.Recipe.Servings))
                .GroupBy(p => p.Key)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));


            return new RecommendationResponseDto
            {
                RecipesAndAmounts = bestCombo
                    .Select(rc => new RecipeWithAmountDto
                    {
                        Recipe = new RecipeShortcutDto { Id = rc.Recipe.Id,TotalServings = rc.Recipe.Servings, Name = rc.Recipe.Title },
                        Amount = rc.Amount
                    }),
                TimeToRespondInMs = sw.ElapsedMilliseconds,
                TotalDistance = totalDistance,
                Nutrients = aggregated
            };
        }

        private Individual CreateRandomIndividual(
            List<List<RecipeShortcutRedis>> candidatesList,
            int numSlots)
        {
            var indiv = new Individual
            {
                Genes = new Gene[numSlots]
            };
            for (int i = 0; i < numSlots; i++)
            {
                var slotCandidates = candidatesList[i];
                if (slotCandidates.Count == 0)
                {
                    indiv.Genes[i] = new Gene { Recipe = null, Amount = 0 };
                    continue;
                }
                var recipe = slotCandidates[_rng.Next(slotCandidates.Count)];
                int amount = _rng.Next(1, Convert.ToInt32(recipe.Servings * 2) + 1);
                indiv.Genes[i] = new Gene { Recipe = recipe, Amount = amount };
            }
            return indiv;
        }

        private float EvaluateFitness(Individual indiv, Dictionary<int, float> expectations)
        {
            float sumSq = 0f;
            var current = new Dictionary<int, float>();
            foreach (var gene in indiv.Genes)
            {
                var recipe = gene.Recipe;
                var amount = gene.Amount;
                if (recipe == null) continue;
                foreach (var kvp in recipe.NutrientAmounts)
                {
                    float contrib = kvp.Value * amount / recipe.Servings;
                    current[kvp.Key] = current.GetValueOrDefault(kvp.Key, 0f) + contrib;
                }
            }
            foreach (var k in expectations.Keys)
            {
                float expVal = expectations[k];
                float actual = current.GetValueOrDefault(k, 0f);
                float diff = expVal - actual;
                sumSq += (diff * diff) / (expVal * expVal);
            }
            return (float)Math.Sqrt(sumSq);
        }

        private Individual TournamentSelect(List<Individual> population, int tournoiSize)
        {
            Individual best = null;
            for (int i = 0; i < tournoiSize; i++)
            {
                var candidate = population[_rng.Next(population.Count)];
                if (best == null || candidate.Fitness < best.Fitness)
                    best = candidate;
            }
            return best;
        }

        private Individual Crossover(Individual parent1, Individual parent2, int numSlots)
        {
            var child = new Individual { Genes = new Gene[numSlots] };
            int crossPoint = _rng.Next(1, numSlots);
            for (int i = 0; i < numSlots; i++)
            {
                var source = (i < crossPoint) ? parent1 : parent2;
                child.Genes[i] = new Gene
                {
                    Recipe = source.Genes[i].Recipe,
                    Amount = source.Genes[i]?.Amount ?? 0
                };
            }
            return child;
        }

        private void Mutate(Individual indiv, List<List<RecipeShortcutRedis>> candidatesList)
        {
            const float recipeMutProb = 0.1f;
            const float amountMutProb = 0.2f;

            for (int i = 0; i < indiv.Genes.Length; i++)
            {
                if (_rng.NextDouble() < recipeMutProb)
                {
                    var slotCandidates = candidatesList[i];
                    if (slotCandidates.Count > 0)
                    {
                        var newRecipe = slotCandidates[_rng.Next(slotCandidates.Count)];
                        indiv.Genes[i].Recipe = newRecipe;

                        indiv.Genes[i].Amount = _rng.Next(1, Convert.ToInt32(newRecipe.Servings*2) + 1);
                    }
                }
                else if (_rng.NextDouble() < amountMutProb && indiv.Genes[i].Recipe != null)
                {
                    float maxAmt = indiv.Genes[i].Recipe.Servings;
                    indiv.Genes[i].Amount = _rng.Next(1, Convert.ToInt32(maxAmt*2) + 1);
                }
            }
        }
    }

    internal static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            return dict.TryGetValue(key, out var val) ? val : defaultValue;
        }
    }
}
