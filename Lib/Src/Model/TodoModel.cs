using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace todocrud.Lib.Src.Model;

public class TodoModel
{

    
    
   
    
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonId]
    public ObjectId Id { get; set; } 
    public string? Title { get; set; } 
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; } 
}