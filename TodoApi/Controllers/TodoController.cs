using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Models;
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

        [HttpGet("{id:int}",Name ="GetTodo")]
        public ActionResult<TodoDTO> GetTodo(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var todo=_db.Todos.FirstOrDefault(u=>u.Id==id);
            return Ok( todo);
        }

        [HttpPost]
        public ActionResult<TodoCreateDTO>CreateTodo([FromBody]TodoCreateDTO createDTO)
        {
            if (_db.Todos.FirstOrDefault(u => u.Name.ToLower() == createDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("CustomError", "TodoName Already exists");
                return BadRequest(ModelState);
            }
            Todo model = new()
            {
               
                Name=createDTO.Name,
                AppointmentDate=createDTO.AppointmentDate,
                Reminder=createDTO.Reminder,

            };
            _db.Todos.Add(model);
            _db.SaveChanges();  

            return CreatedAtRoute("GetTodo", new {id=model.Id},model);
        }
        [HttpDelete("{id:int}",Name ="DeleteTodo")]
        public IActionResult DeleteTodo(int id)
        {
            var todo=_db.Todos.FirstOrDefault(u=>u.Id== id);

            _db.Todos.Remove(todo);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}",Name ="UpdateTodo")]
        public IActionResult UpdateTodo(int id, [FromBody]TodoUpdateDTO updateDTO)
        {
            if(id!=updateDTO.Id)
            {
                return BadRequest();
            }
            Todo todo = new()
            {
                Id = updateDTO.Id,
                Name = updateDTO.Name,
                AppointmentDate = updateDTO.AppointmentDate,
                Reminder = updateDTO.Reminder
            };
            _db.Todos.Update(todo);
            _db.SaveChanges();
            return NoContent();
        }

    }
    

}
