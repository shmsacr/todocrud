namespace todocrud.Lib.Src.DTOs.TodoDTO;

public class ResponseAllTodoDto
{
    public string? Id { get; set; } 
    public string? Title { get; set; } 
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; } 
}