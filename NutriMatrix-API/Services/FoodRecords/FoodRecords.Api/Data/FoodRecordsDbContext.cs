using FoodRecords.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FoodRecords.Api.Data
{
    public class FoodRecordsDbContext:DbContext
    {
        public FoodRecordsDbContext(DbContextOptions<FoodRecordsDbContext> options) : base(options) 
        { }

        public DbSet<FoodRecord> FoodRecords { get; set; }
        public DbSet<RecipeRecord> RecipeRecords { get; set; }

        public DbSet<FoodPlan> FoodPlans { get; set; }

    }
}
