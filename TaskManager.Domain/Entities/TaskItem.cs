using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public TaskStatusEnum Status { get; set; }
    public int? AssigneeId { get; set; }
    public User Assignee { get; set; }
    public DateTime CreatedAt { get; set; }
}