using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using RecommendationService.Api.Models;

namespace RecommendationService.Api.Data
{
    public static class Seeding
    {
        private class IngredientMeasure
        {
            public long food_id { get; set; }
            public long measure_id { get; set; }
            public float unit_amount { get; set; }
        }

        private class FullNutritionData
        {
            public List<IngredientMeasure> Measures { get; set; } = new();
            public Dictionary<int, float> Nutrients { get; set; } = new();
        }

        private class RecipeInfoRow
        {
            public long recipe_id { get; set; }
            public string title { get; set; }  // Already present
            public string description { get; set; }
            public float? servings { get; set; }
            public string directions { get; set; }
            public string photo_url { get; set; }
            public string primary_category_name { get; set; }
        }

        private sealed class RecipeInfoMap : ClassMap<RecipeInfoRow>
        {
            public RecipeInfoMap()
            {
                Map(m => m.recipe_id).Name("recipe_id");
                Map(m => m.title).Name("title"); 
                Map(m => m.description).Name("description");
                Map(m => m.servings).Name("servings");
                Map(m => m.directions).Name("directions");
                Map(m => m.photo_url).Name("photo_url");
                Map(m => m.primary_category_name).Name("primary_category_name");
            }
        }

        public static (List<Recipe> Recipes, List<RecipeMeasure> Measures, List<NutrientAmount> Nutrients)
            GetRecipes()
        {
            var nutritionPath = Path.Combine(AppContext.BaseDirectory, "Assets", "recipe_nutrition_data_sample.csv");
            var infoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "my_recipes_sample.csv");

            var nutritionDict = ParseNutritionCsv(nutritionPath);
            var recipeInfos = ParseRecipeInfoCsv(infoPath);

            var recipes = new List<Recipe>();
            var recipeMeasures = new List<RecipeMeasure>();
            var nutrientAmounts = new List<NutrientAmount>();

            foreach (var info in recipeInfos)
            {
                var recipe = new Recipe
                {
                    Id = info.recipe_id,
                    Title = info.title,  // ADDED TITLE HERE
                    Category = info.primary_category_name,
                    Servings = info.servings,
                    Description = info.description,
                    Directions = info.directions,
                    PhotoUrl = info.photo_url,
                    IsDeleted = false,
                    Measures = new List<RecipeMeasure>(),
                    NutrientsPerTotalServings = new List<NutrientAmount>()
                };

                if (nutritionDict.TryGetValue(info.recipe_id, out var nutritionData))
                {
                    foreach (var measure in nutritionData.Measures)
                    {
                        var rm = new RecipeMeasure
                        {
                            FoodId = measure.food_id,
                            MeasureId = measure.measure_id,
                            RecipeId = info.recipe_id,
                            Amount = measure.unit_amount,
                            IsDeleted = false
                        };
                        recipeMeasures.Add(rm);
                        recipe.Measures.Add(rm);
                    }

                    foreach (var nutrient in nutritionData.Nutrients)
                    {
                        var na = new NutrientAmount
                        {
                            RecipeId = info.recipe_id,
                            NutrientId = nutrient.Key,
                            Amount = nutrient.Value,
                            IsDeleted = false
                        };
                        nutrientAmounts.Add(na);
                        recipe.NutrientsPerTotalServings.Add(na);
                    }
                }

                recipes.Add(recipe);
            }

            return (recipes, recipeMeasures, nutrientAmounts);
        }

        private static Dictionary<long, FullNutritionData> ParseNutritionCsv(string path)
        {
            var nutritionDict = new Dictionary<long, FullNutritionData>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                MissingFieldFound = null,
                HasHeaderRecord = true,
                Mode = CsvMode.RFC4180,
                DetectDelimiter = true,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLower()
            };

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord.ToList();

            while (csv.Read())
            {
                try
                {
                    if (!long.TryParse(csv.GetField("recipe_id"), out long recipeId))
                        continue;

                    var ingredientsJson = csv.GetField("ingredients_clean");
                    var measures = new List<IngredientMeasure>();

                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                        };
                        measures = JsonSerializer.Deserialize<List<IngredientMeasure>>(ingredientsJson, options)
                                   ?? new List<IngredientMeasure>();
                    }
                    catch
                    {
                        // Log error but continue
                    }

                    var nutrients = new Dictionary<int, float>();
                    foreach (var header in headers)
                    {
                        if (header.StartsWith("attr_") &&
                            int.TryParse(header.AsSpan(5), out int nutrientId))
                        {
                            var fieldValue = csv.GetField(header);
                            if (float.TryParse(
                                fieldValue,
                                NumberStyles.Float | NumberStyles.AllowThousands,
                                CultureInfo.InvariantCulture,
                                out float amount))
                            {
                                nutrients[nutrientId] = amount;
                            }
                        }
                    }

                    nutritionDict[recipeId] = new FullNutritionData
                    {
                        Measures = measures,
                        Nutrients = nutrients
                    };
                }
                catch
                {
                    // Skip bad rows
                }
            }

            return nutritionDict;
        }

        private static List<RecipeInfoRow> ParseRecipeInfoCsv(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLower()
            };

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<RecipeInfoMap>();
            var rows = csv.GetRecords<RecipeInfoRow>().ToList();

            foreach (var row in rows)
            {
                row.title = Clean(row.title);
                row.description = Clean(row.description);
                row.directions = Clean(row.directions);
                row.photo_url = Clean(row.photo_url);
                row.primary_category_name = Clean(row.primary_category_name);
            }

            return rows;
        }
        private static string? Clean(string? input)
        {
            return input?.Replace("\0", string.Empty).Trim();
        }

    }
}