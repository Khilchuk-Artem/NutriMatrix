using Ardalis.Specification;
using FoodRecords.Api.Models.Domain;

namespace FoodRecords.Api.Data.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAll(int pageNumber = 1, int pageSize = 5);
        Task<List<T>> GetAll(Specification<T> specification, int pageNumber = 1, int pageSize = 5);
        Task<T> Get(Guid id);
        Task<T> Get(Guid id, Specification<T> specification);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(Guid id);
    }
}
