using MongoDB.Bson;
using todocrud.Lib.Src.DTOs.TodoDTO;
using todocrud.Lib.Src.Model;

namespace todocrud.Lib.Src.Service;

public interface ITodoService
{
    Task<List<ResponseAllTodoDto>> GetAllTodos();
    Task<TodoModel> GetTodoById(ObjectId id);
    Task<TodoModel?> CreateTodoModel(TodoModel todoModel);
    Task <IResult> UpdateTodoModel(ObjectId id, TodoModel todoModel);
    Task <IResult>DeleteTodoModel(ObjectId id);
    Task<List<ResponseAllTodoDto>?> GetTodoByUserId();
    Task<IResult> UpdateTodoStatus(ObjectId id, bool status);
    
}