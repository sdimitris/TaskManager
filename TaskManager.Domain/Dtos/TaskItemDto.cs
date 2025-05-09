using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Dtos;

public class TaskItemDto
{
    public string Title { get; set;} = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AssigneeUsername { get; set; }
    public TaskStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
}