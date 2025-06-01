using CsvHelper.Configuration;
using CsvHelper;
using RecommendationService.Api.Models;
using System.Globalization;
using System.Text.Json;

namespace RecommendationService.Api.Data
{
    public class NutrientCsvRecord
    {
        public int recipe_id { get; set; }
        public string ingredients_clean { get; set; }

        public Dictionary<string, float> Nutrients { get; set; } = new();
    }

    public class MetadataCsvRecord
    {
        public int recipe_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public float rating { get; set; }
        public float? servings { get; set; }
        public string total_time { get; set; }
        public int main_num_ratings { get; set; }
        public string ingredients { get; set; }
        public string directions { get; set; }
        public string photo_url { get; set; }
        public string primary_category_name { get; set; }
    }

    public static class RecipeParser
    {
        public static List<Recipe> ParseRecipes(string nutrientsCsvPath, string metadataCsvPath)
        {
            var nutrients = ParseNutrients(nutrientsCsvPath);
            var metadata = ParseMetadata(metadataCsvPath);

            var result = new List<Recipe>();

            foreach (var meta in metadata)
            {
                var recipe = new Recipe
                {
                    Id = meta.recipe_id,
                    Category = meta.primary_category_name,
                    Description = meta.description,
                    Directions = meta.directions,
                    PhotoUrl = meta.photo_url,
                    Servings = meta.servings,
                    IsDeleted = false,
                    Measures = new List<RecipeMeasure>(),
                    NutrientsPerTotalServings = new List<NutrientAmount>()
                };

                var nutrientEntry = nutrients.FirstOrDefault(n => n.recipe_id == meta.recipe_id);
                if (nutrientEntry != null)
                {
                    var ingredients = JsonSerializer.Deserialize<List<IngredientJson>>(nutrientEntry.ingredients_clean);

                    foreach (var ingredient in ingredients)
                    {
                        ((List<RecipeMeasure>)recipe.Measures).Add(new RecipeMeasure
                        {
                            Amount = ingredient.unit_amount,
                            FoodId = ingredient.food_id,
                            MeasureId = ingredient.measure_id,
                            RecipeId = meta.recipe_id,
                            IsDeleted = false
                        });
                    }

                    foreach (var (key, value) in nutrientEntry.Nutrients)
                    {
                        if (key.StartsWith("attr_") && float.TryParse(value.ToString(), out var amount))
                        {
                            var nutrientId = int.Parse(key.Replace("attr_", ""));
                            ((List<NutrientAmount>)recipe.NutrientsPerTotalServings).Add(new NutrientAmount
                            {
                                NutrientId = nutrientId,
                                Amount = amount,
                                RecipeId = meta.recipe_id,
                                IsDeleted = false
                            });
                        }
                    }
                }

                result.Add(recipe);
            }

            return result;
        }

        private static List<NutrientCsvRecord> ParseNutrients(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            });

            var raw = csv.GetRecords<dynamic>().ToList();

            var list = new List<NutrientCsvRecord>();
            foreach (var row in raw)
            {
                var dict = (IDictionary<string, object>)row;
                var record = new NutrientCsvRecord
                {
                    recipe_id = int.Parse(dict["recipe_id"].ToString()),
                    ingredients_clean = dict["ingredients_clean"].ToString()
                };

                foreach (var kvp in dict)
                {
                    if (kvp.Key.StartsWith("attr_") && float.TryParse(kvp.Value?.ToString(), out var val))
                        record.Nutrients[kvp.Key] = val;
                }

                list.Add(record);
            }

            return list;
        }

        private static List<MetadataCsvRecord> ParseMetadata(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            });

            return csv.GetRecords<MetadataCsvRecord>().ToList();
        }
    }

    public class IngredientJson
    {
        public int food_id { get; set; }
        public int measure_id { get; set; }
        public float unit_amount { get; set; }
    }
}
