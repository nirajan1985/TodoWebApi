using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TodoDTO>> GetTodos()
        {
            return Ok( _db.Todos.ToList());
        }

        [HttpGet("{id:int}",Name ="GetTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<TodoCreateDTO>CreateTodo([FromBody]TodoCreateDTO createDTO)
        {
            if (_db.Todos.FirstOrDefault(u => u.Name.ToLower() == createDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("CustomError", "TodoName Already exists");
                return BadRequest(ModelState);
            }
            Todo todo = new()
            {
               
                Name=createDTO.Name,
                AppointmentDate=createDTO.AppointmentDate,
                Reminder=createDTO.Reminder,

            };
            _db.Todos.Add(todo);
            _db.SaveChanges();  

            return CreatedAtRoute("GetTodo", new {id=todo.Id},todo);
        }
        [HttpDelete("{id:int}",Name ="DeleteTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteTodo(int id)
        {
            var todo=_db.Todos.FirstOrDefault(u=>u.Id== id);

            _db.Todos.Remove(todo);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}",Name ="UpdateTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [HttpPatch("{id:int}",Name ="UpdatePartialTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdatePartialTodo(int id, [FromBody] JsonPatchDocument<TodoUpdateDTO> patchDTO)
        {
            var todo=_db.Todos.AsNoTracking().FirstOrDefault(u=>u.Id== id);
            TodoUpdateDTO updateDTO = new()
            {
                Id = todo.Id,
                Name = todo.Name,
                AppointmentDate = todo.AppointmentDate,
                Reminder= todo.Reminder
            };
            patchDTO.ApplyTo(updateDTO);

            Todo model = new()
            {
                Id = updateDTO.Id,
                Name = updateDTO.Name,
                AppointmentDate = updateDTO.AppointmentDate,
                Reminder = updateDTO.Reminder
            };

            _db.Todos.Update(model);
            _db.SaveChanges();

            return NoContent();
        }

    }
    

}
