using Ardalis.Specification;
using RecommendationService.Api.Models;

namespace RecommendationService.Api.Data.Repository
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAll(int pageNumber = 1, int pageSize = 5);
        Task<List<T>> GetAll(Specification<T> specification, int pageNumber = 1, int pageSize = 5);
        Task<T> Get(long id);
        Task<T> Get(long id, Specification<T> specification);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(long id);
    }
}
