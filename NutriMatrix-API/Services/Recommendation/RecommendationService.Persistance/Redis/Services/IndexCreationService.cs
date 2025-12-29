using Microsoft.Extensions.Hosting;
using RecommendationService.Persistance.Redis.Entities;
using Redis.OM;

namespace RecommendationService.Persistance.Redis.Services
{
    public class IndexCreationService : IHostedService
    {
        private readonly RedisConnectionProvider _provider;
        public IndexCreationService(RedisConnectionProvider provider)
            => _provider = provider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _provider.Connection.CreateIndexAsync(typeof(RecipeShortcutRedis));
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
