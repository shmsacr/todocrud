using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public UserService(IOptions<MongoDBUserSettings> mongoDbSettings) {
        MongoClient client = new MongoClient(mongoDbSettings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _userCollection = database.GetCollection<UserModel>(mongoDbSettings.Value.CollectionName);
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
            return Results.BadRequest("User with this email already exists");
        }

        UserModel user = new UserModel(
            name: newUser.Name,
            email: newUser.Email,
            password: passwordHash
            );
        
        await _userCollection.InsertOneAsync(user);
        return Results.Ok(user);
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
            return Results.BadRequest("User with this email does not exist");
        }
        var verifyPassword = _passwordHasher.Verify(loginDTO.Password, dbUser.Password!);
        if (verifyPassword)
        {
            return Results.Ok(dbUser);
        }
        return Results.BadRequest("Password is incorrect");
    }

}