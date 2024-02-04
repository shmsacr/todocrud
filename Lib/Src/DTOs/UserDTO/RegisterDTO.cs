namespace todocrud.Lib.Src.DTOs.UserDTO;
using System.Text.Json.Serialization;
public class RegisterDTO
{
    [JsonConstructor]
    RegisterDTO(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    
}