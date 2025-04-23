using CsvHelper;
using CsvHelper.Configuration;
using FoodCatalog.Api.Models.Domain;
using System.Globalization;

namespace FoodCatalog.Api.Data.SeedData
{
    public class NutrientCsvRow
    {
        public Guid Id { get; set; }
        public int AttrId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }
    public class NutrientMap : ClassMap<NutrientCsvRow>
    {
        public NutrientMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.AttrId).Name("attr_id");
            Map(m => m.Name).Name("name");
            Map(m => m.Unit).Name("unit");
        }
    }
    public static class SeedDataHelpers
    {
        public static List<Nutrient> LoadNutrients()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "nutrients-mapping.csv");

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<NutrientMap>();

            var records = csv
                .GetRecords<NutrientCsvRow>()
                .Select(r => new Nutrient() { Id = r.Id, Name = r.Name, Unit= r.Unit,IsDeleted=false});

            return records.ToList();
        }
    }
}
