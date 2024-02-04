using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using todocrud.Lib.Src.DTOs.TodoDTO;
using todocrud.Lib.Src.Model;
using todocrud.Lib.Src.Service;

namespace todocrud.Controllers;

[Controller]
[Route("api/[controller]")]
public class TodoController : Controller
{
    // GET
    private readonly TodoService _todoService;
    public TodoController(TodoService todoService)
    {
        _todoService = todoService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTodos()
    {
        return Ok(await _todoService.GetAllTodos());
    }
    
    [HttpGet("getById/")]
    public async Task<IActionResult> GetTodoById(GetDTO id)
    {
        
        var todo = await _todoService.GetTodoById(ObjectId.Parse(id.Id));
        if (todo == null)
        {
            return NotFound();
        }
        return Ok(todo);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTodoModel([FromBody] CreateTodoRequest todoModel)
    {
        var todo = new TodoModel(
            title:todoModel.Title,
            description:todoModel.Description,
            isCompleted: todoModel.İsCompleted
        );  
        await _todoService.CreateTodoModel(todo);
        return CreatedAtAction("GetTodoById", new {id = todo.Id}, todo);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoModel(string id, [FromBody] UpdateDTO todoModel)
    {
        var todo = new TodoModel(
            title:todoModel.Title,
            description:todoModel.Description,
            isCompleted: todoModel.IsCompleted
            
        );
        var result = await _todoService.UpdateTodoModel(ObjectId.Parse(id), todo);
        if (result)
        {
            return Ok();
        }
        return NotFound();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteTodoModel([FromBody]DeleteDTO id)
    {
        
        var result = await _todoService.DeleteTodoModel(ObjectId.Parse(id.Id));
        if (result)
        {
            return Ok();
        }
        return NotFound();
    }
    
}