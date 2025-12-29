using CsvHelper;
using CsvHelper.Configuration;
using FoodCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Persistance.Helpers
{
    public class NutrientCsvRow
    {
        public Guid Id { get; set; }
        public long AttrId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }

    public class FoodCsvRow
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
    }
    public class FoodMap : ClassMap<FoodCsvRow>
    {
        public FoodMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.Name).Name("name");
            Map(m => m.PhotoUrl).Name("photo_url");
        }
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

    public class FoodNutrientCsvRow
    {
        public long FoodId { get; set; }
        public long AttrId { get; set; }
        public float Value { get; set; }
    }

    public class FoodNutrientMap : ClassMap<FoodNutrientCsvRow>
    {
        public FoodNutrientMap()
        {
            Map(m => m.FoodId).Name("food_id");
            Map(m => m.AttrId).Name("attr_id");
            Map(m => m.Value).Name("value");
        }
    }

    public class MeasureCsvRow
    {
        public long Id { get; set; }
        public long FoodId { get; set; }
        public float ServingWeight { get; set; }
        public string Measure { get; set; }
    }

    public class MeasureMap : ClassMap<MeasureCsvRow>
    {
        public MeasureMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.FoodId).Name("food_id");
            Map(m => m.ServingWeight).Name("serving_weight");
            Map(m => m.Measure).Name("measure");
        }
    }


    public static class SeedDataHelpers
    {
        public static IEnumerable<Nutrient> LoadNutrientsMappings()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "nutrients-mapping.csv");

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<NutrientMap>();

            var records = csv
                .GetRecords<NutrientCsvRow>()
                .Select(r => new Nutrient() { Id = r.AttrId, Name = r.Name, Unit = r.Unit, IsDeleted = false }).ToList();

            var hmm = records.Select(r => r.Name).ToList();

            return records;
        }
        public static List<Food> LoadFoods()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "foods.csv");

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<FoodMap>();

            var records = csv
                .GetRecords<FoodCsvRow>()
                .Select(r => new Food
                {
                    Id = r.Id,
                    Name = r.Name,
                    Photo = r.PhotoUrl,
                    IsDeleted = false,
                    FoodNutrients = new List<FoodNutrientIn100g>(),
                    Measures = new List<Measure>()
                });


            return records.ToList();
        }
        public static List<FoodNutrientIn100g> LoadNutrients()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "nutrients.csv");

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<FoodNutrientMap>();

            var records = csv
                .GetRecords<FoodNutrientCsvRow>()
                .Select((r, i) => new FoodNutrientIn100g
                {
                    Id = i + 1,
                    FoodId = r.FoodId,
                    NutrientId = r.AttrId,
                    Amount = r.Value,
                    IsDeleted = false,
                });

            return records.ToList();
        }
        public static List<Measure> LoadMeasures()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "measures.csv");

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<MeasureMap>();

            var records = csv
                .GetRecords<MeasureCsvRow>()
                .Select(r => new Measure
                {
                    Id = r.Id,
                    Name = r.Measure,
                    WeightInGrams = r.ServingWeight,
                    FoodId = r.FoodId,
                    IsDeleted = false,
                });

            return records.ToList();
        }

    }
}
