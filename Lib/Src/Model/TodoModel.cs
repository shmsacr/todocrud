using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace todocrud.Lib.Src.Model;

public class TodoModel
{

    public TodoModel(string title, string description, bool? isCompleted)
    {
        Id = ObjectId.GenerateNewId();
        Title = title;
        Description = description;
        IsCompleted = isCompleted ?? false;
        CreatedAt = DateTime.Now;
    }
    
   
    
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonId]
    public ObjectId Id { get; set; } 
    public string? Title { get; set; } 
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; } 
}