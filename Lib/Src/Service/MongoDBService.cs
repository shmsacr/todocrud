using Microsoft.Extensions.Options;
using MongoDB.Driver;
using todocrud.Lib.Src.Model;

namespace todocrud.Lib.Src.Service;

public class MongoDBService
{
    private readonly IMongoCollection<TodoModel> _todoCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings) {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _todoCollection = database.GetCollection<TodoModel>(mongoDBSettings.Value.CollectionName);
    }
    
}