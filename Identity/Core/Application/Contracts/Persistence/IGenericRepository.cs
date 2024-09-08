using System.Linq.Expressions;

namespace Identity.Core.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAll();
        Task<T> Get(int id);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<bool> IsExist(Expression<Func<T, bool>> expression);
        Task Save();
    }
}
