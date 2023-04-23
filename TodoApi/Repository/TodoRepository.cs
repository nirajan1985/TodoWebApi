using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Repository.IRepository;

namespace TodoApi.Repository
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ApplicationDbContext _db;
        public TodoRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    
        public async Task CreateAsync(Todo entity)
        {
            await _db.Todos.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Todo> GetAsync(Expression<Func<Todo,bool>> filter = null, bool tracked = true)
        {
            IQueryable<Todo> query = _db.Todos;
            if (!tracked)
            {
                query=query.AsNoTracking();
            }
            if (filter != null)
            {
                query=query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Todo>> GetAllAsync(Expression<Func<Todo,bool>> filter = null)
        {
            IQueryable<Todo> query = _db.Todos;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task RemoveAsync(Todo entity)
        {
            _db.Todos.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Todo entity)
        {
            _db.Todos.Update(entity);
            await SaveAsync();
        }
    }
}
