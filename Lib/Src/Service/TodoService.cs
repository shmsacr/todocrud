using System.Security.Claims;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using todocrud.Lib.Src.Model;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Mvc;
using todocrud.Lib.Src.DTOs.TodoDTO;

namespace todocrud.Lib.Src.Service;

public class TodoService: ITodoService
{
    private readonly IMongoCollection<TodoModel> _collection;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserService _userService;
    public TodoService(IOptions<MongoDBSettings> mongoDBService, IHttpContextAccessor httpContextAccessor, UserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        MongoClient client = new MongoClient(mongoDBService.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(mongoDBService.Value.DatabaseName);
        _collection = database.GetCollection<TodoModel>(mongoDBService.Value.CollectionName);
    }
    public async Task<List<ResponseAllTodoDto>> GetAllTodos()
    {
        List<ResponseAllTodoDto> _responseAllTodoDtos = new List<ResponseAllTodoDto>();
        List<TodoModel> todoModels = await _collection.Find(new BsonDocument()).ToListAsync();
        foreach (var todo in todoModels)
        {
            _responseAllTodoDtos.Add(new ResponseAllTodoDto
            {
                Id = todo.Id.ToString(),
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            });
        }

        return _responseAllTodoDtos;
    }

    public async Task<TodoModel> GetTodoById(ObjectId id)
    {
        var finder = await _collection.Find(todo => todo.Id == id).FirstOrDefaultAsync();
        if(finder != null)
        {
            return finder;
        }

        return null;
    }

    public async Task<TodoModel> CreateTodoModel([FromBody] TodoModel todoModel)
    {
        var claimIdentity = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;
        var userId = ObjectId.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
        Console.WriteLine(userId);
        todoModel.CreatedAt = DateTime.Now; 
        await _collection.InsertOneAsync(todoModel);
        await _userService.AddTodoToUser(userId, todoModel.Id);
        return todoModel;
    }

    public async Task <IResult> UpdateTodoModel(ObjectId id, TodoModel todoModel)
    { 
        var finder = await _collection.Find(todo => todo.Id == id).FirstOrDefaultAsync();
        if(finder != null)
        {
           
            await _collection.ReplaceOneAsync(todo => todo.Id.Equals(id), todoModel);
            return Results.Ok(true);
        }
        return Results.BadRequest("Todo not found");
    }

    public async Task<IResult> DeleteTodoModel(ObjectId id)
    {
        var claimIdentity = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;
        var userId = ObjectId.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
        FilterDefinition<TodoModel> filter = Builders<TodoModel>.Filter.Eq("Id", id);
         await _collection.DeleteOneAsync(filter);
        await _userService.RemoveTodoFromUser(userId, id);
        var finder = await _collection.Find(todo => todo.Id == id).FirstOrDefaultAsync();
        if(finder == null)
        {
            return Results.Ok(true);
        }
        return Results.BadRequest("Todo not found");
    }

    public async Task<List<ResponseAllTodoDto>?> GetTodoByUserId()
    {
        var claimIdentity = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;
        var userId = ObjectId.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
        List<ResponseAllTodoDto>? responseAllTodoDtos = [];
        UserModel user = await _userService.GetUserById(userId);

        if(user.TodosRef == null)
        {
            return null;
        }
        user.TodosRef!.ForEach( async todoId =>
        {
            var todo = await GetTodoById(todoId);
            responseAllTodoDtos.Add(new ResponseAllTodoDto
            {
                Id = todo.Id.ToString(),
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            });
        });
        while (responseAllTodoDtos.Count != user.TodosRef.Count)
        {
            await Task.Delay(100);
        }
        return responseAllTodoDtos;
    }

    public async Task<IResult> UpdateTodoStatus(ObjectId id, bool status)
    {
        var todo = await GetTodoById(id);
        todo.IsCompleted = status;
        return Results.Ok(await UpdateTodoModel(id, todo));
    }
}