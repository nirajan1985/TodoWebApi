using System.Linq.Expressions;
using TodoApi.Models;

namespace TodoApi.Repository.IRepository
{
    public interface ITodoRepository
    {
        Task <List<Todo>> GetAllAsync (Expression<Func<Todo,bool>> filter=null);
        Task<Todo> GetAsync(Expression<Func<Todo,bool>> filter = null, bool tracked = true);
        Task CreateAsync(Todo entity);
        Task RemoveAsync(Todo entity);
        Task UpdateAsync(Todo entity);
        Task SaveAsync();
    }
}
