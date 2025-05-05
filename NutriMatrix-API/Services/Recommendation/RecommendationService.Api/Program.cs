
using FoodCatalog.Grpc;
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

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHostedService<IndexCreationService>();

            builder.Services.AddSingleton<RedisConnectionProvider>(new RedisConnectionProvider(builder.Configuration.GetConnectionString("Redis")));
            builder.Services.AddRedisEntityCollection<RecipeShortcutRedis>();
            builder.Services.AddRedisEntityCollection<CategoryAverageNutrientsRedis>();

            builder.Services.AddScoped<INutrientsAnalysisService, NutrientsAnalysisService>();
            builder.Services.AddScoped<IRecipeRecommendationService, RecipeRecommendationService>();
            builder.Services.AddScoped<IQdrantService, QdrantService>();

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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
