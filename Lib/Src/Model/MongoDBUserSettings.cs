namespace todocrud.Lib.Src.Model;

public class MongoDBUserSettings
{
    public string ConnectionUri { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}