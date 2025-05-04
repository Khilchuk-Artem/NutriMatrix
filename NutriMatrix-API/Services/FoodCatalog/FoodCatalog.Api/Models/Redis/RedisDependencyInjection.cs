using Microsoft.Extensions.DependencyInjection.Extensions;
using Redis.OM;
using Redis.OM.Contracts;

namespace FoodCatalog.Api.Models.Redis
{
    public static class RedisDependencyInjection
    {
        public static IServiceCollection AddRedisEntityCollection<T>(this IServiceCollection services) where T:IRedisEntity
        {
            services.AddScoped(sp =>
            {
                var connProvider = sp.GetRequiredService<IRedisConnectionProvider>();

                connProvider.Connection.CreateIndex(typeof(T));

                return connProvider.RedisCollection<T>();
            });

            return services;
        }
    }
}
