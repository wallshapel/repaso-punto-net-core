// Repositories/IRepository.cs
namespace api.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, bool track = true);
        Task<List<T>> GetAllAsync(bool track = false);
        Task<List<TResult>> ProjectAsync<TResult>(
        System.Linq.Expressions.Expression<Func<T, TResult>> selector,
        bool track = false);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> ExistsAsync(int id);
        Task<int> SaveChangesAsync();
    }
}

