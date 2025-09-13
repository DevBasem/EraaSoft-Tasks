using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Repositories.IRepositories;
using System.Linq.Expressions;

namespace MovieApp.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MoviesDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MoviesDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracking = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracking = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync(object id, Expression<Func<T, object>>[]? includes = null)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null && includes != null)
            {
                foreach (var include in includes)
                {
                    await _context.Entry(entity).Reference(include).LoadAsync();
                }
            }

            return entity;
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter == null)
            {
                return await _dbSet.CountAsync();
            }

            return await _dbSet.CountAsync(filter);
        }

        public MoviesDbContext GetDbContext()
        {
            return _context;
        }
    }
}
