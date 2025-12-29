using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using RecommendationService.Api.Services.RecommendationService;
using RecommendationService.Application.Features.Recipes.Queries;
using RecommendationService.Domain.Contracts;
using RecommendationService.Persistance.Context;
using RecommendationService.Persistance.Context.Interceptors;
using RecommendationService.Persistance.Data.Repository;
using RecommendationService.Persistance.Helpers;
using RecommendationService.Persistance.Qdrant;
using RecommendationService.Persistance.Redis.DependencyInjection;
using RecommendationService.Persistance.Redis.Entities;
using RecommendationService.Persistance.Redis.Services;
using Redis.OM;

namespace RecommendationService.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHostedService<IndexCreationService>();

            builder.Services.AddSingleton<RedisConnectionProvider>(new RedisConnectionProvider(builder.Configuration.GetConnectionString("Redis")));
            builder.Services.AddRedisEntityCollection<RecipeShortcutRedis>();

            builder.Services.AddScoped<IRecipeRecommendationService, RecipeRecommendationService>();
            builder.Services.AddScoped<IQdrantService, QdrantService>();
            builder.Services.AddScoped<RecipeRedisSyncInterceptor>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            if (builder.Environment.IsProduction())
            {
                var portVar = Environment.GetEnvironmentVariable("PORT");
                if (portVar is { Length: > 0 } && int.TryParse(portVar, out var port))
                {
                    builder.WebHost.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(port);
                    });
                }
            }
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetRecipeShortcutsQuery).Assembly));

            builder.Services.AddDbContext<RecipeDbContext>((sp, options) =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("RecommendationService.Persistance"));
                options.AddInterceptors(sp.GetRequiredService<RecipeRedisSyncInterceptor>());
            });

            builder.Services.AddSingleton<QdrantClient>(_ =>
            {
                var qdrantConn = builder.Configuration.GetConnectionString("Qdrant");
                var parts = qdrantConn.Split(':');
                var host = parts[0];
                var port = int.Parse(parts[1]);

                return new QdrantClient(host, port);
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
                //db.Database.Migrate();
                if (!db.Recipes.Any())
                {
                    var (recipes, recipeMeasures, nutrientAmounts) = Seeding.GetRecipes();

                    var non161Nutrients = recipes.Where(r => nutrientAmounts.Where(na => na.RecipeId == r.Id).Count() != 161).ToList();


                    foreach (var recipeChunk in recipes.Chunk(100))
                    {
                        await db.Recipes.AddRangeAsync(recipeChunk);
                        await db.SaveChangesAsync(); 
                    }
                }

            }

            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
            });

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
