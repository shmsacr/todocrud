using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using todocrud.Lib.Src.Model;

namespace todocrud.Lib.Src.Service;

public class TodoService: ITodoService
{
    private readonly IMongoCollection<TodoModel> _collection;
    public TodoService(IOptions<MongoDBSettings> mongoDBService)
    {
        MongoClient client = new MongoClient(mongoDBService.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(mongoDBService.Value.DatabaseName);
        _collection = database.GetCollection<TodoModel>(mongoDBService.Value.CollectionName);
    }
    public async Task<List<TodoModel>> GetAllTodos()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
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

    public async Task<TodoModel> CreateTodoModel(TodoModel todoModel)
    {
        todoModel.CreatedAt = DateTime.Now;
        await _collection.InsertOneAsync(todoModel);
        return todoModel;
    }

    public async Task <bool> UpdateTodoModel(ObjectId id, TodoModel todoModel)
    { 
        var finder = await _collection.Find(todo => todo.Id == id).FirstOrDefaultAsync();
        if(finder != null)
        {
            todoModel.CreatedAt = finder.CreatedAt;
            await _collection.ReplaceOneAsync(todo => todo.Id == id, todoModel);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteTodoModel(ObjectId id)
    {
        FilterDefinition<TodoModel> filter = Builders<TodoModel>.Filter.Eq("Id", id);
        DeleteResult deleteResult = await _collection.DeleteOneAsync(filter);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }
}