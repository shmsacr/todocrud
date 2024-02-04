namespace todocrud.Lib.Src.DTOs.TodoDTO;

public class CreateTodoRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? İsCompleted { get; set; } = false;
    
    
}