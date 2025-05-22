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

            var connstring = "Host=localhost;Port=1334;Database=FoodCatalogDb;Username=postgres;Password=postgres;";
            var tmp = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<FoodCatalogDbContext>();
            optionsBuilder.UseNpgsql(connstring);

            return new FoodCatalogDbContext(optionsBuilder.Options);
        }
    }
}
