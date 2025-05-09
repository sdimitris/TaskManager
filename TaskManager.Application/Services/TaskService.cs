using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.Requests;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IUserService _userService;
    public TaskService(IUserService userService, ITaskItemRepository taskRepository)
    {
        ArgumentNullException.ThrowIfNull(_taskRepository = taskRepository);
        ArgumentNullException.ThrowIfNull(_userService = userService);
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

    public async Task<Result<TaskItem>> CreateTaskAsync(CreateTaskItemRequest task)
    {
            
        var taskToAdd = new TaskItem
        {
            Title = task.Title,
            Description = task.Description,
            Status = TaskStatusEnum.Todo,
            AssigneeId = null,
            Assignee = null,
            CreatedAt = DateTime.UtcNow,
        };

        if (task.AssigneeUsername is not null)
        {
            var assignee = await _userService.GetUserByUsername(task.AssigneeUsername);
            if (assignee.IsFailure)
            {
                return Result<TaskItem>.FromFailure(assignee);
            }

            if (assignee.Value is null)
            {
                return Result<TaskItem>.Failure(Error.New($"Can not assign ticket to user with {task.AssigneeUsername} becauase the user does not exist",null,KnownApplicationErrorEnum.UserNotFound));
            }
            
            taskToAdd.AssigneeId = assignee.Value!.Id;
            taskToAdd.Assignee = assignee.Value;
        }
        
        var addResult = await _taskRepository.AddAsync(taskToAdd);
        if (addResult.IsFailure)
        {
            return Result<TaskItem>.Failure(addResult.Error);
        }
        
        return Result<TaskItem>.Ok(taskToAdd);
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