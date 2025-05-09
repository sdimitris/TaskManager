using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Requests;

namespace TaskManager.Domain.Interfaces;

public interface ITaskService
{
    Task<Result<IEnumerable<TaskItem>>> GetAllTasksAsync();
    Task<Result<TaskItem>> GetTaskByIdAsync(int id);
    Task<Result<TaskItem>> CreateTaskAsync(CreateTaskItemRequest task);
    Task<Result> UpdateTaskAsync(TaskItem task);
    Task<Result> DeleteTaskAsync(int id);
}