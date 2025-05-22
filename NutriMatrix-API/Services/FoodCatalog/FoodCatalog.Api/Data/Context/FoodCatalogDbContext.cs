using FoodCatalog.Api.Data.SeedData;
using FoodCatalog.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;


namespace FoodCatalog.Api.Data.Context
{
    public class FoodCatalogDbContext : DbContext
    {
        public FoodCatalogDbContext(DbContextOptions<FoodCatalogDbContext> options) : base(options)
        { }

        public DbSet<Food> Foods { get; set; }
        public DbSet<Measure> Measures { get; set; }
        public DbSet<Nutrient> Nutrients { get; set; }
        //public DbSet<FoodMeal> FoodMeals { get; set; }
        //public DbSet<Meal> Meals { get; set; }
        public DbSet<FoodNutrientIn100g> FoodNutrientIn100Gs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /*builder.Entity<Nutrient>().HasData(SeedDataHelpers.LoadNutrientsMappings());
            builder.Entity<Food>().HasData(SeedDataHelpers.LoadFoods());
            builder.Entity<FoodNutrientIn100g>().HasData(SeedDataHelpers.LoadNutrients());
            builder.Entity<Measure>().HasData(SeedDataHelpers.LoadMeasures());*/
        }
    }
}
