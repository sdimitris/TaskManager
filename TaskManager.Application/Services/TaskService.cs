using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskItemRepository _taskRepository;

    public TaskService(ITaskItemRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<IEnumerable<TaskItem>>> GetAllTasksAsync()
    {
        
        var tasksResult = await _taskRepository.GetAllAsync();
        if (tasksResult.IsFailure)
        {
            Result<IEnumerable<TaskItem>>.Failure(tasksResult.Error);
        }
        
        return tasksResult;
    }

    public async Task<Result<TaskItem>> GetTaskByIdAsync(int id)
    {
        var getTaskResult =  await _taskRepository.GetByIdAsync(id);
        if (getTaskResult.IsFailure)
        {
            return Result<TaskItem>.Failure(getTaskResult.Error);
        }
        if (getTaskResult.Value is null)
        {
            return Result<TaskItem>.Failure(Error.New($"Task with id {id} not found", null, KnownApplicationErrorEnum.TaskNotFound));
        }
        
        return Result<TaskItem>.Ok(getTaskResult.Value);
    }

    public async Task<Result<TaskItem>> CreateTaskAsync(TaskItem task)
    {
        task.CreatedAt = DateTime.UtcNow;
        var addResult = await _taskRepository.AddAsync(task);
        if (addResult.IsFailure)
        {
            return Result<TaskItem>.Failure(addResult.Error);
        }
        
        return Result<TaskItem>.Ok(task);
    }

    public async Task<Result> UpdateTaskAsync(TaskItem task)
    {
        var existingTask = await _taskRepository.GetByIdAsync(task.Id);
        if (existingTask.IsFailure)
            return Result.Failure(existingTask.Error);
        
        if (existingTask.Value is null)
            return Result.Failure(Error.New($"Task with id {task.Id} not found", null, KnownApplicationErrorEnum.TaskNotFound));
                
        existingTask.Value.Title = task.Title;
        existingTask.Value.Description = task.Description;
        existingTask.Value.Status = task.Status;
        existingTask.Value.AssigneeId = task.AssigneeId;
        
        var updateResult = await _taskRepository.UpdateAsync(existingTask.Value);
        if (updateResult.IsFailure)
            return Result.Failure(updateResult.Error);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteTaskAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task.IsFailure)
            return Result.Failure(task.Error);
        
        if (task.Value is null)
            return Result.Failure(Error.New($"Task with id {id} not found", null, KnownApplicationErrorEnum.TaskNotFound));

        var deleteResult = await _taskRepository.DeleteAsync(id);
        
        if (deleteResult.IsFailure)
            return Result.Failure(deleteResult.Error);
        
        return Result.Ok();
    }
}