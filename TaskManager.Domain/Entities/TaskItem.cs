namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public Guid? AssigneeId { get; set; }
    public User Assignee { get; set; }
    public DateTime CreatedAt { get; set; }
}