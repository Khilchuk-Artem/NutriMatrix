
using BuildingBlocks.Nutrionix.Refit;
using BuildingBlocks.Nutrionix.DI;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Contracts;
using Redis.OM;
using FoodCatalog.Api.Models.Redis;
using FoodCatalog.Api.Data.Interceptors;
using FoodCatalog.Api.Data.DbContext;
namespace FoodCatalog.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddNutrionixApi();

            builder.Services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(builder.Configuration.GetConnectionString("Redis")));
            builder.Services.AddRedisEntityCollection<FoodRedis>();
            builder.Services.AddSingleton<FoodRedisSyncInterceptor>();

            builder.Services.AddDbContext<FoodCatalogDbContext>(options =>
                {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

                    options.AddInterceptors(builder.Services.BuildServiceProvider().GetRequiredService<FoodRedisSyncInterceptor>());
                });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FoodCatalogDbContext>();
                db.Database.Migrate();
            }

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
