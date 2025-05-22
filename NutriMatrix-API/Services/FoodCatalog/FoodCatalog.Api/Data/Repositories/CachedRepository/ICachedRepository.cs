using Ardalis.Specification;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Redis;

namespace FoodCatalog.Api.Data.Repositories.CachedRepository
{
    public interface ICachedRepository<T> where T : IRedisEntity
    {
        Task<List<T>> GetAll(int pageNumber = 1, int pageSize = 5);
        Task<List<T>> GetAll(Specification<T> specification, int pageNumber = 1, int pageSize = 5);
        Task<T> Get(long id);
        Task<T> Get(long id, Specification<T> specification);
        Task<T> Add(T entity);
        Task<T> Delete(long id);
    }
}
