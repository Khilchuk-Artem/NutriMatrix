
using BuildingBlocks.Nutrionix.Refit;
using BuildingBlocks.Nutrionix.DI;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Contracts;
using Redis.OM;
using FoodCatalog.Api.Models.Redis;
using FoodCatalog.Api.Data.Interceptors;
using FoodCatalog.Api.Data.Context;
using Microsoft.EntityFrameworkCore.Diagnostics;
using FoodCatalog.Api.Data.SeedData;
using System.Text.Json.Serialization;
using FoodCatalog.Api.Models.Domain;
using BuildingBlocks.Messaging.Extensions;
using FoodCatalog.Api.Integration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using FoodCatalog.Api.Data.Repositories.Repository;

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
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddDbContext<FoodCatalogDbContext>((sp, options) =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.AddInterceptors(sp.GetRequiredService<FoodRedisSyncInterceptor>());
                options.ConfigureWarnings(w =>
                    w.Ignore(RelationalEventId.PendingModelChangesWarning));
            });

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumers(typeof(GetMealByIdConsumer).Assembly);

                x.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(builder.Configuration["MessageBroker:UserName"]);
                        host.Password(builder.Configuration["MessageBroker:Password"]);
                    });

                    configurator.ConfigureEndpoints(context);
                });

            });
                
        

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                DatabaseHelper.CreateDatabaseIfNotExists("FoodCatalogDb");

                var db = scope.ServiceProvider.GetRequiredService<FoodCatalogDbContext>();

                db.Database.Migrate();

                if (!db.Foods.Any())
                {
                    var foods = SeedDataHelpers.LoadFoods();

                    var allMeasures = SeedDataHelpers.LoadMeasures();
                    var allNutrients = SeedDataHelpers.LoadNutrients();

                    Parallel.ForEach(foods, food =>
                    {
                        food.Measures = allMeasures.Where(m => m.FoodId == food.Id).ToList();
                        food.FoodNutrients = allNutrients.Where(n => n.FoodId == food.Id).ToList();
                    });

                    foreach (var food in foods)
                    {
                        await db.Foods.AddAsync(food);

                        await db.SaveChangesAsync();
                    }
                }
                
            }

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
