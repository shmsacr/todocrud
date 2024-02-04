using MongoDB.Bson;
using todocrud.Lib.Src.Model;

namespace todocrud.Lib.Src.Service;

public interface ITodoService
{
    Task<List<TodoModel>> GetAllTodos();
    Task<TodoModel> GetTodoById(ObjectId id);
    Task<TodoModel?> CreateTodoModel(TodoModel todoModel);
    Task <bool> UpdateTodoModel(ObjectId id, TodoModel todoModel);
    Task <bool>DeleteTodoModel(ObjectId id);
    
}