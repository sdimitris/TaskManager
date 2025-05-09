using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Dtos;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Requests;

namespace TaskManager.Domain.Interfaces;

public interface ITaskService
{
    Task<Result<IEnumerable<TaskItemDto>>> GetAllTasksAsync();
    Task<Result<TaskItemDto>> GetTaskByIdAsync(int id);
    Task<Result<TaskItemDto>> CreateTaskAsync(CreateTaskItemRequest task);
    Task<Result> UpdateTaskAsync(TaskItem task);
    Task<Result> DeleteTaskAsync(int id);
}