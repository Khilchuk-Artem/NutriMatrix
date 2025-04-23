using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using FoodCatalog.Api.Models.Domain;
using Microsoft.EntityFrameworkCore.Storage;
using FoodCatalog.Api.Models.Redis;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Data.Repositories.CachedRepository
{
    public class CachedRepository<T> : ICachedRepository<T> where T : class, IRedisEntity
    {
        private readonly IRedisCollection<T> _collection;
        public CachedRepository(IRedisCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<T> Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _collection.InsertAsync(entity);

            return entity;
        }

        public async Task<T> Delete(Guid id)
        {
            var entity = await _collection.FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null) return entity;

            await _collection.DeleteAsync(entity);

            return entity;
        }

        public Task<T> Get(Guid id)
        {
            return _collection.FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<T> Get(Guid id, Specification<T> specification)
        {
            return ApplySpecifications(_collection.AsQueryable(), specification)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<List<T>> GetAll(int pageNumber = 1, int pageSize = 5)
        {
            if (pageNumber < 1 || pageSize < 1) return Task.FromResult(new List<T>());
            return _collection
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<List<T>> GetAll(Specification<T> specification, int pageNumber = 1, int pageSize = 5)
        {
            if (pageNumber < 1 || pageSize < 1) return Task.FromResult(new List<T>());
            return ApplySpecifications(_collection.AsQueryable(), specification)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private IQueryable<T> ApplySpecifications(IQueryable<T> inputQuery, Specification<T> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(inputQuery, specification);
        }
    }
}
