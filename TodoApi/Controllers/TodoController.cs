using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
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
        protected APIResponse _response;
        public TodoController(ITodoRepository dbTodo, IMapper mapper)
        {
            _dbTodo = dbTodo;
            _mapper= mapper;
            this._response= new();
        }
       

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< ActionResult<APIResponse>> GetTodos()
        {
            try
            {
                IEnumerable<Todo> todoList = await _dbTodo.GetAllAsync();
                _response.Result = _mapper.Map<List<TodoDTO>>(todoList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}",Name ="GetTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< ActionResult<APIResponse>> GetTodo(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var todo = await _dbTodo.GetAsync(u => u.Id == id);
                _response.Result = _mapper.Map<TodoDTO>(todo);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task< ActionResult<APIResponse>>CreateTodo([FromBody]TodoCreateDTO createDTO)
        {
            try
            {
                if (await _dbTodo.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "TodoName Already exists");
                    return BadRequest(ModelState);
                }

                Todo todo = _mapper.Map<Todo>(createDTO);

                await _dbTodo.CreateAsync(todo);
                _response.Result = _mapper.Map<TodoCreateDTO>(todo);
                _response.StatusCode = HttpStatusCode.Created;


                return CreatedAtRoute("GetTodo", new { id = todo.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete("{id:int}",Name ="DeleteTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteTodo(int id)
        {
            try
            {
                var todo = await _dbTodo.GetAsync(u => u.Id == id);

                await _dbTodo.RemoveAsync(todo);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id:int}",Name ="UpdateTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateTodo(int id, [FromBody]TodoUpdateDTO updateDTO)
        {
            try
            {

                if (id != updateDTO.Id)
                {
                    return BadRequest();
                }

                Todo todo = _mapper.Map<Todo>(updateDTO);

                await _dbTodo.UpdateAsync(todo);
                _response.Result = _mapper.Map<TodoUpdateDTO>(todo);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPatch("{id:int}",Name ="UpdatePartialTodo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdatePartialTodo(int id, [FromBody] JsonPatchDocument<TodoUpdateDTO> patchDTO)
        {
            try
            {
                var todo = await _dbTodo.GetAsync(u => u.Id == id, tracked: false);

                TodoUpdateDTO updateDTO = _mapper.Map<TodoUpdateDTO>(todo);
                patchDTO.ApplyTo(updateDTO);



                Todo model = _mapper.Map<Todo>(updateDTO);

                await _dbTodo.UpdateAsync(model);
                _response.Result = _mapper.Map<TodoUpdateDTO>(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

    }
    

}
