using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using todocrud.Lib.Src.DTOs.TodoDTO;
using todocrud.Lib.Src.Model;
using todocrud.Lib.Src.Service;

namespace todocrud.Lib.Src.EnpointHandlers;

public class TodoEndpointHandlers
{
    private readonly ITodoService _todoService;
    
    public TodoEndpointHandlers(ITodoService todoService)
    {
        _todoService = todoService;
    }
    
    public async Task<List<TodoModel>> GetAllTodos()
    {
        return await _todoService.GetAllTodos();
    }
    public async Task<IResult> GetTodoById(string id)
    {
        var todo = await _todoService.GetTodoById(ObjectId.Parse(id));
        if (todo == null)
        {
            return Results.BadRequest("Todo not found");
        }
        return Results.Ok(todo);
    }
    public async Task<IResult> CreateTodoModel([FromBody]CreateTodoRequest todoModel)
    {
        var todo = new TodoModel(
            title:todoModel.Title,
            description:todoModel.Description
            
        );  
        await _todoService.CreateTodoModel(todo);
        return Results.Ok(todo);
    }
    public async Task<bool> UpdateTodoModel(string id, UpdateDTO todoModel)
    {
        var todo = new TodoModel(
            title:todoModel.Title,
            description:todoModel.Description
        );
        return await _todoService.UpdateTodoModel(ObjectId.Parse(id), todo);
    }
    public async Task<bool> DeleteTodoModel(string id)
    {
        return await _todoService.DeleteTodoModel(ObjectId.Parse(id));
    }
    
    
}