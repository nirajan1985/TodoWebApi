using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Models.Dto;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route ("api/Todo")]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TodoController(ApplicationDbContext db)
        {
            _db= db;
        }
       

        [HttpGet]
        public ActionResult<IEnumerable<TodoDTO>> GetTodos()
        {
            return Ok( _db.Todos.ToList());
        }
    }
}
