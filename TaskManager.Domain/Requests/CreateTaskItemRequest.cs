namespace TaskManager.Domain.Requests;

public class CreateTaskItemRequest
{
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AssigneeUsername { get; set; }
}