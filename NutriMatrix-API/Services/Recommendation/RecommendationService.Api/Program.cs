
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
        public static void Main(string[] args)
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

            builder.Services.AddDbContext<RecipeDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
