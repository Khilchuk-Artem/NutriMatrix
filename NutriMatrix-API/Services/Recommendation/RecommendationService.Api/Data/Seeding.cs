using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using RecommendationService.Api.Models.Redis;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Redis.OM;
using Vector = Redis.OM.Vector;
namespace RecommendationService.Api.Data
{
    public class IngredientClean
    {
        public string food_name { get; set; }
        public string unit_name { get; set; }
        public float unit_amount { get; set; }
    }

    public class RecipeInfoRow
    {
        public long recipe_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string directions { get; set; }
        public float? servings { get; set; }
        public string primary_category_name { get; set; }

        public List<IngredientClean> ingredients_clean { get; set; }
    }

    public class IngredientCleanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return JsonSerializer.Deserialize<List<IngredientClean>>(text);
        }
    }

    public sealed class RecipeInfoMap : ClassMap<RecipeInfoRow>
    {
        public RecipeInfoMap()
        {
            Map(m => m.recipe_id).Name("recipe_id");
            Map(m => m.servings).Name("servings");
            Map(m => m.description).Name("description");
            Map(m => m.directions).Name("directions");
            Map(m => m.primary_category_name).Name("primary_category_name");
            Map(m => m.ingredients_clean)
                .Name("ingredients_clean")
                .TypeConverter<IngredientCleanConverter>();
        }
    }
    public static class Seeding
    {
        static Dictionary<long, Dictionary<int, float>> LoadNutrition(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                Quote = '"',
                Escape = '"',
                Mode = CsvMode.RFC4180,
                BadDataFound = null,
                MissingFieldFound = null
            };

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<dynamic>()
                             .Cast<IDictionary<string, object>>()
                             .ToList();

            var result = new Dictionary<long, Dictionary<int, float>>();
            foreach (var row in records)
            {
                var id = long.Parse((string)row["recipe_id"]);
                var nutrients = new Dictionary<int, float>();

                foreach (var kv in row)
                {
                    if (kv.Key.StartsWith("attr_") &&
                        float.TryParse((string)kv.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    {
                        var nutId = int.Parse(kv.Key.Substring(5));
                        nutrients[nutId] = val;
                    }
                }

                result[id] = nutrients;
            }

            return result;
        }
        static List<RecipeShortcutRedis> BuildShortcuts(
            List<RecipeInfoRow> infos,
            Dictionary<long, Dictionary<int, float>> nutData)
        {
            return infos.Select(info => new RecipeShortcutRedis
            {
                Id = info.recipe_id,
                Servings = info.servings ?? 0f,
                Category = info.primary_category_name,
                IngredientIds = null,
                NutrientAmounts = nutData.TryGetValue(info.recipe_id, out var dict2)
                                         ? dict2
                                         : new Dictionary<int, float>()
            }).Where(r=>r.NutrientAmounts.Count==161).ToList();
        }
        static List<RecipeInfoRow> LoadInfo(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<RecipeInfoMap>();
            return csv.GetRecords<RecipeInfoRow>().ToList();
        }

        public static IEnumerable<RecipeShortcutRedis> GetRecipeShortcutRedis()
        {
            var nutrition = LoadNutrition(Path.Combine(AppContext.BaseDirectory, "Assets", "recipe_nutrition_data.csv"));
            var infos = LoadInfo(Path.Combine(AppContext.BaseDirectory, "Assets", "my_recipes.csv"));
            return BuildShortcuts(infos, nutrition);
        }
    }
}
