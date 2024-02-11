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
    
    public async Task<List<ResponseAllTodoDto>> GetAllTodos()
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
        var todo = new TodoModel()
        {
            Id = ObjectId.GenerateNewId(),
            Title = todoModel.Title,
            Description = todoModel.Description,
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
        await _todoService.CreateTodoModel(todo);
        return Results.Ok(todo);
    }
    public async Task<IResult> UpdateTodoModel( UpdateDTO todoModel)
    {
        var todo = new TodoModel()
        {
            Id = ObjectId.Parse(todoModel.Id),
            Title = todoModel.Title,
            Description = todoModel.Description,
            IsCompleted = todoModel.IsCompleted,
            CreatedAt = DateTime.Now
        };
        return await _todoService.UpdateTodoModel(ObjectId.Parse(todoModel.Id), todo);
    }
    public async Task<IResult> DeleteTodoModel(string id)
    {
        return await _todoService.DeleteTodoModel(ObjectId.Parse(id));
    }
    
    public async Task<List<ResponseAllTodoDto>?> getTodoByUserId()
    {
        return await _todoService.GetTodoByUserId();
    }
    public async Task<IResult> updateTodoStatus(string id, bool status)
    {
        return await _todoService.UpdateTodoStatus(ObjectId.Parse(id), status);
    }
}