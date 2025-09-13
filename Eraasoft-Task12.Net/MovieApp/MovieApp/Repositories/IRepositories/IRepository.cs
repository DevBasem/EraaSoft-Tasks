using System.Linq.Expressions;

namespace MovieApp.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracking = true);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracking = true);

        Task<T?> GetByIdAsync(object id, Expression<Func<T, object>>[]? includes = null);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

        MovieApp.DataAccess.MoviesDbContext GetDbContext();
    }
}
