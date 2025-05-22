using Microsoft.EntityFrameworkCore;
using RecommendationService.Api.Models;

namespace RecommendationService.Api.Data
{
    public class RecipeDbContext : DbContext
    {
        public RecipeDbContext(DbContextOptions<RecipeDbContext> options):base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeMeasure> RecipeMeasure { get; set; }
        public DbSet<NutrientAmount> NutrientAmounts { get; set; }


    }
}
