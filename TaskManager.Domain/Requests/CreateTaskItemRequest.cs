namespace TaskManager.Domain.Requests;

public class CreateTaskItemRequest
{
    /// <summary>
    /// Title of the task
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Description of the task
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Assignee username of the task ( can be null)
    /// </summary>
    public string? AssigneeUsername { get; set; }
}