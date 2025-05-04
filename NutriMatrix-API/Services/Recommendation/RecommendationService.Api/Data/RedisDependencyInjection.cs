using RecommendationService.Api.Models.Redis;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace RecommendationService.Api.Data
{
    public static class RedisDependencyInjection
    {
        public static IServiceCollection AddRedisEntityCollection<T>(this IServiceCollection services) where T : class
        {
            services.AddScoped(sp =>
            {
                var connProvider = sp.GetRequiredService<RedisConnectionProvider>();

                connProvider.Connection.CreateIndex(typeof(T));

                return (RedisCollection<T>)connProvider.RedisCollection<T>();
            });

            return services;
        }
    }
}
