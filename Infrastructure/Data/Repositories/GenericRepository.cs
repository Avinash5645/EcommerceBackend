using Application.DTOs;
using Core.Comman;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().Where(predicate).ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }
        public async Task<PagedResultDto<T>> GetPagedAsync(PaginationParams paginationParams, Expression<Func<T, bool>>? filter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                                   .Take(paginationParams.PageSize)
                                   .ToListAsync();

            return new PagedResultDto<T>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public IQueryable<T> AsQueryable() => _context.Set<T>().AsQueryable();

    }
}
