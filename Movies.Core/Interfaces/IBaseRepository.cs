
namespace Movies.Core.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id, string[] includes = null!);
        Task<IEnumerable<T>> GetAllAsync(string[] includes = null!);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null!);

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null!);

        Task<T> AddAsync(T entity);

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        void Attach(T entity);
        void AttachRange(IEnumerable<T> entities);

        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
    }
}
