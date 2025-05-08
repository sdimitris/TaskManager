using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces;

public interface ITaskService
{
    Task<Result<IEnumerable<TaskItem>>> GetAllTasksAsync();
    Task<Result<TaskItem>> GetTaskByIdAsync(int id);
    Task<Result<TaskItem>> CreateTaskAsync(TaskItem task);
    Task<Result> UpdateTaskAsync(TaskItem task);
    Task<Result> DeleteTaskAsync(int id);
}