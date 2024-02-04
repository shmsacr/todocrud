using MongoDB.Bson;
using todocrud.Lib.Src.DTOs.UserDTO;
using todocrud.Lib.Src.Model;
using todocrud.Lib.Src.Service;

namespace todocrud.Lib.Src.EnpointHandlers;

public class UserEndpointHandlers
{
    private readonly IUserService _userService;
    public UserEndpointHandlers(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<List<UserModel>> GetAllUsers()
    {
        return await _userService.GetAllUsers();
    }
    public async Task<IResult> GetUserById(string id)
    {
        var user = await _userService.GetUserById(ObjectId.Parse(id));
        if (user == null)
        {
            return Results.BadRequest("User not found");
        }
        return Results.Ok(user);
    }
    public async Task<IResult> Register(RegisterDTO userModel)
    {
        return await _userService.Register(userModel);
    }
    public async Task<bool> UpdateUser(string id, UserModel userModel)
    {
        return await _userService.UpdateUser(ObjectId.Parse(id), userModel);
    }
    public async Task<bool> DeleteUser(string id)
    {
        return await _userService.DeleteUser(ObjectId.Parse(id));
    }
    public async Task<IResult> LoginUser(LoginDTO loginDTO)
    {
        return await _userService.LoginUser(loginDTO);
    }
    
}