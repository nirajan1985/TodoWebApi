using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Models.Dto;
using TodoApi.Repository.IRepository;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route ("api/Todo")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _dbTodo;
        private readonly IMapper _mapper;
        public TodoController(ITodoRepository dbTodo, IMapper mapper)
        {
            _dbTodo = dbTodo;
            _mapper= mapper;
        }
       

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< ActionResult<IEnumerable<TodoDTO>>> GetTodos()
        {
            IEnumerable<Todo> todoList = await _dbTodo.GetAllAsync();
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
            var todo=await _dbTodo.GetAsync(u=>u.Id==id);
            return Ok( _mapper.Map<TodoDTO>(todo));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task< ActionResult<TodoCreateDTO>>CreateTodo([FromBody]TodoCreateDTO createDTO)
        {
            if (await _dbTodo.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("CustomError", "TodoName Already exists");
                return BadRequest(ModelState);
            }
            
           Todo todo= _mapper.Map<Todo>(createDTO);

            await _dbTodo.CreateAsync(todo);
            await _dbTodo.SaveAsync();  

            return CreatedAtRoute("GetTodo", new {id=todo.Id},todo);
        }
        [HttpDelete("{id:int}",Name ="DeleteTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task< IActionResult> DeleteTodo(int id)
        {
            var todo=await _dbTodo.GetAsync(u=>u.Id== id);

            await _dbTodo.RemoveAsync(todo);
            await _dbTodo.SaveAsync();
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
           
            Todo todo=_mapper.Map<Todo>(updateDTO);

            await _dbTodo.UpdateAsync(todo);
            await _dbTodo.SaveAsync();
            return NoContent();
        }
        [HttpPatch("{id:int}",Name ="UpdatePartialTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< IActionResult> UpdatePartialTodo(int id, [FromBody] JsonPatchDocument<TodoUpdateDTO> patchDTO)
        {
            var todo=await _dbTodo.GetAsync(u=>u.Id== id,tracked:false);
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

            await _dbTodo.UpdateAsync(model);
           await _dbTodo.SaveAsync();

            return NoContent();
        }

    }
    

}
