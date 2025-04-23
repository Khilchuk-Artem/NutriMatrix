using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Data.Context
{
    public class FoodCatalogDbContextFactory : IDesignTimeDbContextFactory<FoodCatalogDbContext>
    {
        public FoodCatalogDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FoodCatalogDbContext>();
            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));

            return new FoodCatalogDbContext(optionsBuilder.Options);
        }
    }
}
