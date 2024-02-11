using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.WireProtocol.Messages;
using todocrud.Lib.Src.DTOs.UserDTO;
using todocrud.Lib.Src.Model;

namespace todocrud.Lib.Src.Service;

public class UserService: IUserService
{
    private readonly IMongoCollection<UserModel> _userCollection;
    private readonly PasswordHasher _passwordHasher = new PasswordHasher();
    private readonly TokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(IOptions<MongoDBUserSettings> mongoDbSettings, TokenService tokenService, IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
        MongoClient client = new MongoClient(mongoDbSettings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _userCollection = database.GetCollection<UserModel>(mongoDbSettings.Value.CollectionName);
        _tokenService = tokenService;
    }
    public async Task<UserModel> GetUserById(ObjectId id)
    {
        var finder = await _userCollection.Find(user => user.Id == id).FirstOrDefaultAsync(); 
        if(finder != null)
        {
            return finder;
        }
        return null;
        
    }

    public async Task<IResult> Register(RegisterDTO newUser)
    {
        if(newUser.Email == null || newUser.Password == null)
        {
            return Results.BadRequest("Email and Password cannot be null");

        }
        string passwordHash =  _passwordHasher.Hash(newUser.Password);
        var finder = await _userCollection.Find(user => user.Email == newUser.Email).FirstOrDefaultAsync();
        if(finder != null)
        {
            return Results.BadRequest("User already exists");
        }

        UserModel user = new UserModel(
            name: newUser.Name,
            email: newUser.Email,
            password: passwordHash
            );
        
        await _userCollection.InsertOneAsync(user);
        var tokenHandler = _tokenService.GenerateToken(user);
        ResponseLoginDto responseLoginDto = new ResponseLoginDto()
        {
            Id = user.Id.ToString(),
            Token = tokenHandler,
            Name = user.Name,
        };
        return Results.Ok(responseLoginDto);
    }

    public async Task<bool> UpdateUser(ObjectId id, UserModel userModel)
    {
        var finder = await _userCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
        if(finder != null)
        {
            userModel.CreatedAt = finder.CreatedAt;
            await _userCollection.ReplaceOneAsync(user => user.Id == id, userModel);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteUser(ObjectId id)
    {
        FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Eq("Id", id);
        DeleteResult deleteResult = await _userCollection.DeleteOneAsync(filter);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        
    }

    public Task<UserModel> GetUserByEmail(string email)
    {
        var finder = _userCollection.Find(user => user.Email == email).FirstOrDefault();
        if (finder != null)
        {
            return Task.FromResult(finder);
        }
        return null;
    }

    public Task<List<UserModel>> GetAllUsers()
    {
        return _userCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<IResult> LoginUser(LoginDTO loginDTO)
    {
        if (loginDTO.Email == null || loginDTO.Password == null)
        {
            return Results.BadRequest("Email and Password cannot be null");
        }

        var dbUser = await _userCollection.Find(user => user.Email == loginDTO.Email).FirstOrDefaultAsync();
        if (dbUser == null)
        { 
            return Results.NotFound("User not found");
        }
        var verifyPassword = _passwordHasher.Verify(loginDTO.Password, dbUser.Password!);
        if (verifyPassword)
        {
            var tokenHandler = _tokenService.GenerateToken(dbUser);
            ResponseLoginDto responseLoginDto = new ResponseLoginDto()
            {
                Id = dbUser.Id.ToString(),
                Token = tokenHandler,
                Name = dbUser.Name,
            };
            
            return Results.Ok(responseLoginDto);
        }
        return Results.Unauthorized();
    }
    
    public async Task<bool> AddTodoToUser(ObjectId userId, ObjectId todoId)
    {
        var userFilter = Builders<UserModel>.Filter.Eq(u => u.Id, userId);
        var user = await _userCollection.Find(userFilter).FirstOrDefaultAsync();
        if (user != null)
        {
            if(user.TodosRef == null)
            {
                user.TodosRef = new List<ObjectId>();
            }
            user.TodosRef.Add(todoId);

            var update = Builders<UserModel>.Update.Set(u => u.TodosRef, user.TodosRef);
            var updateResult = await _userCollection.UpdateOneAsync(userFilter, update);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        return false;
    }

    public async Task<bool> RemoveTodoFromUser(ObjectId userId, ObjectId todoId)
    {
        var userFilter = Builders<UserModel>.Filter.Eq(u => u.Id, userId);
        var user = _userCollection.Find(userFilter).FirstOrDefault();
        if (user != null)
        {
            user.TodosRef.Remove(todoId);
            var update = Builders<UserModel>.Update.Set(u => u.TodosRef, user.TodosRef);
            var updateResult = await _userCollection.UpdateOneAsync(userFilter, update);

            return true;
        }

        return false;
    }

    public async Task<IResult> Logout()
    {
       
        try
        {
             _httpContextAccessor.HttpContext.Response.Headers.Remove("Authorization");
        }catch(Exception e)
        {
            return Results.BadRequest(e.Message);
        }
        
        
        return Results.Ok("Logged out successfully");
    }
}