
using BuildingBlocks.Nutrionix.Refit;
using BuildingBlocks.Nutrionix.DI;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Contracts;
using Redis.OM;
using FoodCatalog.Api.Models.Redis;
using FoodCatalog.Api.Data.Interceptors;
using FoodCatalog.Api.Data.Context;
using FoodCatalog.Grpc;
using FoodCatalog.Api.Services.NutrientsGrpc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using FoodCatalog.Api.Data.SeedData;
using System.Text.Json.Serialization;
using FoodCatalog.Api.Models.Domain;

namespace FoodCatalog.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                }); ;

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddNutrionixApi();
            builder.Services.AddGrpc();
            builder.Services.AddHostedService<IndexCreationService>();

            builder.Services.AddSingleton<RedisConnectionProvider>(new RedisConnectionProvider(builder.Configuration.GetConnectionString("Redis")));
            builder.Services.AddRedisEntityCollection<FoodRedis>();
            builder.Services.AddRedisEntityCollection<MeasureRedis>();
            builder.Services.AddScoped<FoodRedisSyncInterceptor>();

            builder.Services.AddDbContext<FoodCatalogDbContext>((sp, options) =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.AddInterceptors(sp.GetRequiredService<FoodRedisSyncInterceptor>());
                options.ConfigureWarnings(w =>
                    w.Ignore(RelationalEventId.PendingModelChangesWarning));
            });


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FoodCatalogDbContext>();

                //db.Database.Migrate();

                /*await db.FoodNutrientIn100Gs.ExecuteDeleteAsync();
                await db.Measures.ExecuteDeleteAsync();
                await db.Foods.ExecuteDeleteAsync();
                await db.Nutrients.ExecuteDeleteAsync();*/

                //await db.Nutrients.AddRangeAsync(SeedDataHelpers.LoadNutrientsMappings());
                //await db.SaveChangesAsync();

               /* var kek = new Random();

                var milk = new Food
                {
                    // Id is left at 0 so EF Core will generate it
                    Id=2222232222+ kek.Next(-1000001, 100000),
                    Name = "Natural roasted ground coffee Ethiopia Dallmayr, vacuum-packed, 500g",
                    Photo = "https://icf.listex.info/med/9a5f19de-0603-8c5a-4c4c-2519eed5e41c.jpg",
                    IsDeleted = false,
                    Barcode = "4008167504009",

                    Measures = new List<Measure>
                {
            new Measure
            {
                // Id = 0,
                Id=112812343224+ kek.Next(-1000001,100000),
                Name          = "cup",
                WeightInGrams = 250,
                IsDeleted     = false
                // FoodId will be set by EF once milk.Id is known
                    }
                },

                            FoodNutrients = new List<FoodNutrientIn100g>
                {
                    new FoodNutrientIn100g {Id=112123123+ kek.Next(-1000001,100000), NutrientId = 203, Amount =  8.2228f, IsDeleted = false }, // Protein
                    new FoodNutrientIn100g {Id=1121321223+ kek.Next(-1000001,100000), NutrientId = 204, Amount =  2.3668f, IsDeleted = false }, // Total Fat
                    new FoodNutrientIn100g {Id=1123112244+ kek.Next(-1000001,100000), NutrientId = 205, Amount = 12.1756f, IsDeleted = false }, // Carbs
                    new FoodNutrientIn100g {Id=1123112234444+ kek.Next(-1000001,100000), NutrientId = 208, Amount =102.48f,   IsDeleted = false }, // Calories
                    new FoodNutrientIn100g {Id=1112312324114445+ kek.Next(-1000001,100000), NutrientId = 307, Amount =107.36f,   IsDeleted = false }, // Sodium
                    new FoodNutrientIn100g {Id=1123123442111149+ kek.Next(-1000001,100000), NutrientId = 306, Amount =366f,      IsDeleted = false }  // Potassium
                }
                        };

                await db.Foods.AddAsync(milk);
                await db.SaveChangesAsync();*/

                if (!db.Foods.Any())
                {
                    var foods = SeedDataHelpers.LoadFoods();

                    var allMeasures = SeedDataHelpers.LoadMeasures();
                    var allNutrients = SeedDataHelpers.LoadNutrients();

                    foreach (var food in foods)
                    {

                        food.Measures = allMeasures.Where(m => m.FoodId == food.Id).ToList();
                        food.FoodNutrients = allNutrients.Where(n => n.FoodId == food.Id).ToList();
                    }

                    foreach (var food in foods)
                    {
                        await db.Foods.AddAsync(food);

                        await db.SaveChangesAsync();
                    }
                }
                
            }

            app.MapGrpcService<FoodGrpcService>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
