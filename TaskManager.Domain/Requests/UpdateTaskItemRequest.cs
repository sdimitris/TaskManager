using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Requests;

public class UpdateTaskItemRequest
{
    public string? Title { get; set;}
    public string? Description { get; set; }
    public string? AssigneeUsername { get; set; }
    public TaskStatusEnum? Status { get; set; }
}