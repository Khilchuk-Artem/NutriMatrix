using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCatalog.Persistance.Context
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

            builder.Entity<Nutrient>().HasData(SeedDataHelpers.LoadNutrientsMappings());
        }
    }
}
