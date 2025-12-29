using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FoodRecords.Domain.Common;
using FoodRecords.Domain.Contracts;
using FoodRecords.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecords.Persistance.Repository.Implementations
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly FoodRecordsDbContext _context;
        public Repository(FoodRecordsDbContext context)
        {
            _context = context;
        }
        public async Task<T> Add(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Delete(long id)
        {
            var entity = await _context.Set<T>().Where(t => !t.IsDeleted).FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null) return entity;

            entity.IsDeleted = true;

            await _context.SaveChangesAsync();
            return entity;
        }

        public Task<T> Get(long id, Specification<T> specification)
        {
            return ApplySpecifications(_context.Set<T>().AsQueryable(), specification)
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<T> Get(long id)
        {
            return _context
                .Set<T>()
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<List<T>> GetAll(int pageNumber = 1, int pageSize = 5)
        {
            if (pageNumber < 1 || pageSize < 1) return Task.FromResult(new List<T>());
            return _context
                .Set<T>()
                .Where(t => !t.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<List<T>> GetAll(Specification<T> specification, int pageNumber = 1, int pageSize = 5)
        {
            if (pageNumber < 1 || pageSize < 1) return Task.FromResult(new List<T>());
            return ApplySpecifications(_context.Set<T>().AsQueryable(), specification)
                .Where(t => !t.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<T> Update(T entity)
        {
            if (!_context.Set<T>().Local.Any(t => t.Id == entity.Id))
            {
                return null;
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        private IQueryable<T> ApplySpecifications(IQueryable<T> inputQuery, Specification<T> specification)
        {
            return SpecificationEvaluator.Default.GetQuery(inputQuery, specification);
        }
    }
}
