using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using RecommendationService.Api.Models.Redis;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace RecommendationService.Api.Data
{
    public class IngredientClean
    {
        public string food_name { get; set; }
        public string unit_name { get; set; }
        public float unit_amount { get; set; }
    }

    // 2) Info POCO, with a custom converter for that JSON column:
    public class RecipeInfoRow
    {
        public long recipe_id { get; set; }
        public string title { get; set; }
        public float? servings { get; set; }  // <-- allow nulls
        public string primary_category_name { get; set; }

        // we tell CsvHelper to use our JSON converter here:
        public List<IngredientClean> ingredients_clean { get; set; }
    }

    public class IngredientCleanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return JsonSerializer.Deserialize<List<IngredientClean>>(text);
        }
    }

    // 3) Map only the columns we care about:
    public sealed class RecipeInfoMap : ClassMap<RecipeInfoRow>
    {
        public RecipeInfoMap()
        {
            Map(m => m.recipe_id).Name("recipe_id");
            Map(m => m.servings).Name("servings");
            Map(m => m.primary_category_name).Name("primary_category_name");
            /*Map(m => m.ingredients_clean)
                .Name("ingredients_clean")
                .TypeConverter<IngredientCleanConverter>();*/
        }
    }
    public static class Seeding
    {
        static Dictionary<long, Dictionary<int, float>> LoadNutrition(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",       // fields separated by commas
                Quote = '"',       // text‑qualifier
                Escape = '"',       // inner quotes are doubled
                Mode = CsvMode.RFC4180,
                BadDataFound = null,       // ignore malformed lines
                MissingFieldFound = null        // ignore missing fields
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
                NutrientAmounts = nutData.TryGetValue(info.recipe_id, out var dict)
                                         ? dict
                                         : new Dictionary<int, float>()
            }).ToList();
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
