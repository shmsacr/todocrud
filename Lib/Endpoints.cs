using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using todocrud.Lib.Src.DTOs.TodoDTO;
using todocrud.Lib.Src.DTOs.UserDTO;
using todocrud.Lib.Src.EnpointHandlers;
using todocrud.Lib.Src.Model;
using todocrud.Lib.Src.Service;

namespace todocrud.Lib;

public class Endpoints
{
    public static void EnpointsRouter(WebApplication app)
    {
        #region UserEndpoints
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
        user.MapPost("logout", async (UserService userService) =>
        {
            return await new UserEndpointHandlers(userService).Logout();
        }).RequireAuthorization();
        #endregion 
        /////////////////////////////////////////////////////////////////////////////
        #region TodoEndpoints
        var todo = app.MapGroup("api/todos");
        todo.MapGet("getAllTodos", async (TodoService todoService) =>
        {
            return await new TodoEndpointHandlers(todoService).GetAllTodos();
        }).RequireAuthorization();
        todo.MapGet("getTodoById/{id}", async (TodoService todoService, string id) =>
        {
            return await new TodoEndpointHandlers(todoService).GetTodoById(id);
        }).RequireAuthorization();
        todo.MapPost("createTodo", async (TodoService todoService,[FromBody] CreateTodoRequest todoModel) =>
        {
            return await new TodoEndpointHandlers(todoService).CreateTodoModel(todoModel);
        }).RequireAuthorization();
        
        todo.MapPut("updateTodo/", async (TodoService todoService,[FromBody] UpdateDTO todoModel) =>
        {
            return await new TodoEndpointHandlers(todoService).UpdateTodoModel(todoModel);
        }).RequireAuthorization();
        todo.MapDelete("deleteTodo/", async (TodoService todoService, [FromBody] DeleteDTO id) =>
        {
            return await new TodoEndpointHandlers(todoService).DeleteTodoModel(id.Id);
        }).RequireAuthorization();
        
        todo.MapGet("getTodoByUserId/", async (TodoService todoService) =>
        {
            return await new TodoEndpointHandlers(todoService).getTodoByUserId();
        }).RequireAuthorization();
        
        todo.MapPut("updateTodoStatus/", async (TodoService todoService,[FromBody] UpdataTodoStatusDto updateStatus) =>
        {
            return await new TodoEndpointHandlers(todoService).updateTodoStatus(updateStatus.Id, updateStatus.IsCompleted);
        }).RequireAuthorization();
        #endregion

    }
}