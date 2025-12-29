using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodRecords.Persistance.Data;

namespace FoodRecords.Persistance.Context
{
    internal class FoodRecordsDbContextFactory : IDesignTimeDbContextFactory<FoodRecordsDbContext>
    {
        public FoodRecordsDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FoodRecordsDbContext>();
            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));

            return new FoodRecordsDbContext(optionsBuilder.Options);
        }
    }
}
