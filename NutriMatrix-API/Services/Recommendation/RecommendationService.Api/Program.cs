
using FoodCatalog.Grpc;
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

            var kek = Seeding.GetRecipes();

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

            builder.Services.AddGrpcClient<FoodService.FoodServiceClient>(options =>
            {
                options.Address = new Uri(builder.Configuration["GrpcSettings:FoodServiceUri"]!);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return handler;
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
                var hmmm = !db.Recipes.Any();
                if (hmmm)
                {
                    var (recipes, recipeMeasures, nutrientAmounts) = Seeding.GetRecipes();

                    foreach (var recipe in recipes)
                    {
                        recipe.Measures = recipeMeasures
                            .Where(m => m.RecipeId == recipe.Id)
                            .ToList();

                        recipe.NutrientsPerTotalServings = nutrientAmounts
                            .Where(n => n.RecipeId == recipe.Id)
                            .ToList();
                    }

                    foreach (var recipe in recipes)
                    {
                        await db.Recipes.AddAsync(recipe);
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
