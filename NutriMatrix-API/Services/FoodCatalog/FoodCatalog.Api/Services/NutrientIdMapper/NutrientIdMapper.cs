using CsvHelper;
using FoodCatalog.Api.Data.SeedData;
using System.Globalization;

namespace FoodCatalog.Api.Services.NutrientIdMapper
{
    public class NutrientIdMapper : INutrientIdMapper
    {
        private readonly Dictionary<int, Guid> _mappings;

        public NutrientIdMapper(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "Data", "SeedData", "nutrients-mapping.csv");

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<NutrientCsvRow>().ToList();
            _mappings = records.ToDictionary(r => r.AttrId, r => r.Id);
        }

        public Guid? GetGuidForAttrId(int attrId)
        {
            return _mappings.TryGetValue(attrId, out var guid) ? guid : null;
        }
    }
}
