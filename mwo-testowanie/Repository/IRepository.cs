using System.Linq.Expressions;

namespace mwo_testowanie.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}