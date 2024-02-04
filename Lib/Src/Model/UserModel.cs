using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace todocrud.Lib.Src.Model;

public class UserModel
{
    public UserModel(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
        CreatedAt = DateTime.Now;
    }
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonId]
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    [JsonIgnore]
    public string? Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ObjectId>? TodosRef { get; set; }
    
    public string? PasswordHash { get; set; }
    
    
}