using Microsoft.EntityFrameworkCore;
using Qdrant.Client;
using RecommendationService.Api.Data;
using RecommendationService.Api.Models.Dto;
using RecommendationService.Api.Models.Redis;
using RecommendationService.Api.Services.NutrientsAnalysisService;
using RecommendationService.Api.Services.Qdrant;
using RecommendationService.Api.Services.RecommendationService;
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
            builder.Services.AddRedisEntityCollection<CategoryAverageNutrientsRedis>();

            builder.Services.AddScoped<INutrientsAnalysisService, NutrientsAnalysisService>();
            builder.Services.AddScoped<IRecipeRecommendationService, RecipeRecommendationService>();
            builder.Services.AddScoped<IQdrantService, QdrantService>();
            builder.Services.AddScoped<RecipeRedisSyncInterceptor>();

            builder.Services.AddDbContext<RecipeDbContext>((sp, options) =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.AddInterceptors(sp.GetRequiredService<RecipeRedisSyncInterceptor>());
            });

            builder.Services.AddSingleton<QdrantClient>(_ =>
            {
                return new QdrantClient("qdrant", 6334);
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
                db.Database.Migrate();
                if (!db.Recipes.Any())
                {
                    var (recipes, recipeMeasures, nutrientAmounts) = Seeding.GetRecipes();

                    var non161Nutrients = recipes.Where(r => nutrientAmounts.Where(na => na.RecipeId == r.Id).Count() != 161).ToList();
                    if (non161Nutrients.Count > 0)
                    {
                        var hmmmm = 1;
                    }

                    /*Parallel.ForEach(recipes, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, recipe =>
                    {
                        recipe.Measures = recipeMeasures
                            .Where(m => m.RecipeId == recipe.Id)
                            .ToList();

                        recipe.NutrientsPerTotalServings = nutrientAmounts
                            .Where(n => n.RecipeId == recipe.Id)
                            .ToList();
                    });*/


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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
