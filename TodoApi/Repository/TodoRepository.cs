using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Repository.IRepository;

namespace TodoApi.Repository
{
    public class TodoRepository : Repository<Todo>, ITodoRepository
    {
        private readonly ApplicationDbContext _db;
        public TodoRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }
    
        

        public async Task<Todo> UpdateAsync(Todo entity)
        {
            _db.Todos.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
