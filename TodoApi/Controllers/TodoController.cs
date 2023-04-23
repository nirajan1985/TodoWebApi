using AutoMapper;
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
        private readonly IMapper _mapper;
        public TodoController(ApplicationDbContext db, IMapper mapper)
        {
            _db= db;
            _mapper= mapper;
        }
       

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< ActionResult<IEnumerable<TodoDTO>>> GetTodos()
        {
            IEnumerable<Todo> todoList = await _db.Todos.ToListAsync();
            return Ok(_mapper.Map<List<TodoDTO>>(todoList));
        }

        [HttpGet("{id:int}",Name ="GetTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< ActionResult<TodoDTO>> GetTodo(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var todo=await _db.Todos.FirstOrDefaultAsync(u=>u.Id==id);
            return Ok( _mapper.Map<TodoDTO>(todo));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task< ActionResult<TodoCreateDTO>>CreateTodo([FromBody]TodoCreateDTO createDTO)
        {
            if (await _db.Todos.FirstOrDefaultAsync(u => u.Name.ToLower() == createDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("CustomError", "TodoName Already exists");
                return BadRequest(ModelState);
            }
            //Todo todo = new()
            //{
               
            //    Name=createDTO.Name,
            //    AppointmentDate=createDTO.AppointmentDate,
            //    Reminder=createDTO.Reminder,

            //};
           Todo todo= _mapper.Map<Todo>(createDTO);

            await _db.Todos.AddAsync(todo);
            await _db.SaveChangesAsync();  

            return CreatedAtRoute("GetTodo", new {id=todo.Id},todo);
        }
        [HttpDelete("{id:int}",Name ="DeleteTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task< IActionResult> DeleteTodo(int id)
        {
            var todo=await _db.Todos.FirstOrDefaultAsync(u=>u.Id== id);

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}",Name ="UpdateTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< IActionResult> UpdateTodo(int id, [FromBody]TodoUpdateDTO updateDTO)
        {
            if(id!=updateDTO.Id)
            {
                return BadRequest();
            }
            //Todo todo = new()
            //{
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    AppointmentDate = updateDTO.AppointmentDate,
            //    Reminder = updateDTO.Reminder
            //};
            Todo todo=_mapper.Map<Todo>(updateDTO);

            _db.Todos.Update(todo);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id:int}",Name ="UpdatePartialTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< IActionResult> UpdatePartialTodo(int id, [FromBody] JsonPatchDocument<TodoUpdateDTO> patchDTO)
        {
            var todo=await _db.Todos.AsNoTracking().FirstOrDefaultAsync(u=>u.Id== id);
            //TodoUpdateDTO updateDTO = new()
            //{
            //    Id = todo.Id,
            //    Name = todo.Name,
            //    AppointmentDate = todo.AppointmentDate,
            //    Reminder= todo.Reminder
            //};
            TodoUpdateDTO updateDTO=_mapper.Map<TodoUpdateDTO>(todo);
            patchDTO.ApplyTo(updateDTO);

            //Todo model = new()
            //{
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    AppointmentDate = updateDTO.AppointmentDate,
            //    Reminder = updateDTO.Reminder
            //};
            Todo model=_mapper.Map<Todo>(updateDTO);

            _db.Todos.Update(model);
           await _db.SaveChangesAsync();

            return NoContent();
        }

    }
    

}
