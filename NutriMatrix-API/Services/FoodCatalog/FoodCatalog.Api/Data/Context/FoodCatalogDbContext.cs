using BuildingBlocks.Nutrionix.Models;
using FoodCatalog.Api.Data.SeedData;
using FoodCatalog.Api.Models.Domain;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<FoodMeal> FoodMeals { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<FoodNutrientIn100g> FoodNutrientIn100Gs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Nutrient>().HasData(SeedDataHelpers.LoadNutrients());
        }
    }
}
