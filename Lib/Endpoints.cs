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
        todo.MapPut("updateTodo/{id}", async (TodoService todoService, string id, UpdateDTO todoModel) =>
        {
            return await new TodoEndpointHandlers(todoService).UpdateTodoModel(id, todoModel);
        }).RequireAuthorization();
        todo.MapDelete("deleteTodo/{id}", async (TodoService todoService, string id) =>
        {
            return await new TodoEndpointHandlers(todoService).DeleteTodoModel(id);
        }).RequireAuthorization();
        #endregion

    }
}