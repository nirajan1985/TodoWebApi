using System.Linq.Expressions;
using TodoApi.Models;

namespace TodoApi.Repository.IRepository
{
    public interface ITodoRepository:IRepository<Todo>
    {
        
        Task <Todo> UpdateAsync(Todo entity);
        
    }
}
