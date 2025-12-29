using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;
using System.Collections.Concurrent;
using RecommendationService.Domain.Entities;
using RecommendationService.Persistance.Redis.Entities;

namespace RecommendationService.Persistance.Context.Interceptors
{
    public class RecipeRedisSyncInterceptor : SaveChangesInterceptor
    {
        private readonly RedisCollection<RecipeShortcutRedis> _recipeShortcutCollection;
        private readonly ConcurrentDictionary<DbContext, (HashSet<long> toUpdate, HashSet<long> toDelete)> _pendingOperations = new();

        public RecipeRedisSyncInterceptor(
            RedisCollection<RecipeShortcutRedis> recipeShortcutCollection)
        {
            _recipeShortcutCollection = recipeShortcutCollection;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            CaptureChanges(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            CaptureChanges(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void CaptureChanges(DbContext context)
        {
            if (context == null) return;

            var recipesToUpdate = new HashSet<long>();
            var recipesToDelete = new HashSet<long>();

            foreach (var entry in context.ChangeTracker.Entries<Recipe>())
            {
                if (entry.State == EntityState.Added)
                {
                    recipesToUpdate.Add(entry.Entity.Id);
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.IsDeleted)
                    {
                        recipesToDelete.Add(entry.Entity.Id);
                    }
                    else
                    {
                        recipesToUpdate.Add(entry.Entity.Id);
                    }
                }
                else if (entry.State == EntityState.Deleted)
                {
                    recipesToDelete.Add(entry.Entity.Id);
                }
            }

            ProcessRelatedEntities<RecipeMeasure>(context, recipesToUpdate, recipesToDelete);
            ProcessRelatedEntities<NutrientAmount>(context, recipesToUpdate, recipesToDelete);

            recipesToUpdate.ExceptWith(recipesToDelete);

            if (recipesToUpdate.Count > 0 || recipesToDelete.Count > 0)
            {
                _pendingOperations[context] = (recipesToUpdate, recipesToDelete);
            }
        }

        private void ProcessRelatedEntities<T>(
            DbContext context,
            HashSet<long> recipesToUpdate,
            HashSet<long> recipesToDelete) where T : class
        {
            foreach (var entry in context.ChangeTracker.Entries<T>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                {
                    if (entry.Entity is RecipeMeasure rm && !recipesToDelete.Contains(rm.RecipeId))
                    {
                        recipesToUpdate.Add(rm.RecipeId);
                    }
                    else if (entry.Entity is NutrientAmount na && !recipesToDelete.Contains(na.RecipeId))
                    {
                        recipesToUpdate.Add(na.RecipeId);
                    }
                }
            }
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            ProcessRedisUpdates(eventData.Context).Wait();
            return base.SavedChanges(eventData, result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            await ProcessRedisUpdates(eventData.Context, cancellationToken);
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private async Task ProcessRedisUpdates(DbContext context, CancellationToken cancellationToken = default)
        {
            if (context == null || !_pendingOperations.TryRemove(context, out var operations))
            {
                return;
            }

            var (recipesToUpdate, recipesToDelete) = operations;

            try
            {
                if (recipesToUpdate.Count > 0)
                {
                    var recipes = await context.Set<Recipe>()
                        .Where(r => recipesToUpdate.Contains(r.Id) && !r.IsDeleted)
                        .Include(r => r.Measures)
                        .Include(r => r.NutrientsPerTotalServings)
                        .ToListAsync(cancellationToken);

                    foreach (var recipe in recipes)
                    {
                        await UpdateRedisShortcut(recipe);
                    }
                }

                foreach (var recipeId in recipesToDelete)
                {
                    await DeleteRedisShortcut(recipeId);
                }
            }
            catch
            {
            }
        }

        private async Task UpdateRedisShortcut(Recipe recipe)
        {
            var shortcut = await _recipeShortcutCollection.FindByIdAsync(recipe.Id.ToString())
                           ?? new RecipeShortcutRedis { Id = recipe.Id, RecipeId = recipe.Id };

            shortcut.Title = recipe.Title;
            shortcut.Servings = recipe.Servings ?? 0f;
            shortcut.Category = recipe.Category ?? "";
            shortcut.IngredientIds = recipe.Measures?
                .Select(m => m.FoodId)
                .Distinct()
                .ToList() ?? new List<long>();

            shortcut.NutrientAmounts = recipe.NutrientsPerTotalServings?
                .ToDictionary(n => n.NutrientId, n => n.Amount)
                ?? new Dictionary<int, float>();

            await _recipeShortcutCollection.InsertAsync(shortcut);
        }

        private async Task DeleteRedisShortcut(long recipeId)
        {
            var shortcut = await _recipeShortcutCollection.FindByIdAsync(recipeId.ToString());
            if (shortcut != null)
            {
                await _recipeShortcutCollection.DeleteAsync(shortcut);
            }
        }
    }
}
