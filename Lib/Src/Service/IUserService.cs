using MongoDB.Bson;
using todocrud.Lib.Src.DTOs.UserDTO;
using todocrud.Lib.Src.Model;

namespace todocrud.Lib.Src.Service;

public interface IUserService
{
    public Task<UserModel?> GetUserById(ObjectId id);
    public Task<IResult> Register(RegisterDTO userModel);
    public Task<bool> UpdateUser(ObjectId id, UserModel userModel);
    public Task<bool> DeleteUser(ObjectId id);
    public Task<UserModel> GetUserByEmail(string email);
    public Task<List<UserModel>> GetAllUsers();
    public Task<IResult> LoginUser(LoginDTO loginDTO);
    public Task<bool> AddTodoToUser(ObjectId userId, ObjectId todoId);
    
    
}