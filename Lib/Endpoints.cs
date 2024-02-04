using MongoDB.Bson;
using todocrud.Lib.Src.DTOs.UserDTO;
using todocrud.Lib.Src.EnpointHandlers;
using todocrud.Lib.Src.Model;
using todocrud.Lib.Src.Service;

namespace todocrud.Lib;

public class Endpoints
{
    public static void EnpointsRouter(WebApplication app)
    {
        var user = app.MapGroup("api/users");
        user.MapGet("getAllUsers", async (UserService userService) =>
        {
            return await new UserEndpointHandlers(userService).GetAllUsers();
        }).RequireAuthorization("Admin");
        user.MapPost("register", async (UserService userService, RegisterDTO newUser) =>
        {
            return await new UserEndpointHandlers(userService).Register(newUser);
        }).AllowAnonymous();
        user.MapPost("login", async (UserService userService, LoginDTO loginDTO) =>
        {
            return await new UserEndpointHandlers(userService).LoginUser(loginDTO);
        }).AllowAnonymous();
    }
}