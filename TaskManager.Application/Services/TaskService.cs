using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Dtos;
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

    public async Task<Result<IEnumerable<TaskItemDto>>> GetAllTasksAsync()
    {
        var tasksResult = await _taskRepository.GetAllAsync();
        if (tasksResult.IsFailure)
        {
            Result<IEnumerable<TaskItemDto>>.Failure(tasksResult.Error);
        }

        var mappedListToDto = tasksResult.Value.Select(MapToTaskItemDto);

        return Result<IEnumerable<TaskItemDto>>.Ok(mappedListToDto);
    }

    public async Task<Result<TaskItemDto>> GetTaskByIdAsync(int id)
    {
        var getTaskResult = await _taskRepository.GetByIdAsync(id);
        if (getTaskResult.IsFailure)
        {
            return Result<TaskItemDto>.Failure(getTaskResult.Error);
        }

        if (getTaskResult.Value is null)
        {
            return Result<TaskItemDto>.Failure(Error.New($"Task with id {id} not found", null,
                KnownApplicationErrorEnum.TaskNotFound));
        }

        return Result<TaskItemDto>.Ok(new TaskItemDto() { });
    }

    public async Task<Result<TaskItemDto>> CreateTaskAsync(CreateTaskItemRequest task)
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
                return Result<TaskItemDto>.FromFailure(assignee);
            }

            if (assignee.Value is null)
            {
                return Result<TaskItemDto>.Failure(Error.New(
                    $"Can not assign ticket to user with {task.AssigneeUsername} becauase the user does not exist",
                    null, KnownApplicationErrorEnum.UserNotFound));
            }

            taskToAdd.AssigneeId = assignee.Value!.Id;
            taskToAdd.Assignee = assignee.Value;
        }

        var addResult = await _taskRepository.AddAsync(taskToAdd);
        if (addResult.IsFailure)
        {
            return Result<TaskItemDto>.Failure(addResult.Error);
        }

        return Result<TaskItemDto>.Ok(MapToTaskItemDto(taskToAdd));
    }

    public async Task<Result> UpdateTaskAsync(int id, UpdateTaskItemRequest taskItem)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask.IsFailure)
            return Result.Failure(existingTask.Error);

        if (existingTask.Value is null)
            return Result.Failure(Error.New($"Task with id {id} not found", null,
                KnownApplicationErrorEnum.TaskNotFound));

        if (taskItem.AssigneeUsername is not null)
        {
            var assigneeResult = await _userService.GetUserByUsername(taskItem.AssigneeUsername);
            if (assigneeResult.IsFailure)
                return Result.Failure(assigneeResult.Error);

            if (assigneeResult.Value is null)
            {
                return Result.Failure(Error.New(
                    $"Can not attach assignee with username {taskItem.AssigneeUsername} because does not exist", null,
                    KnownApplicationErrorEnum.UserNotFound));
            }

            existingTask.Value.AssigneeId = assigneeResult.Value.Id;
        }

        existingTask.Value.Title = taskItem.Title ?? existingTask.Value.Title;
        existingTask.Value.Description = taskItem.Description ?? existingTask.Value.Description;
        existingTask.Value.Status = taskItem.Status ?? existingTask.Value.Status;

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
            return Result.Failure(Error.New($"Task with id {id} not found", null,
                KnownApplicationErrorEnum.TaskNotFound));

        var deleteResult = await _taskRepository.DeleteAsync(id);

        if (deleteResult.IsFailure)
            return Result.Failure(deleteResult.Error);

        return Result.Ok();
    }

    private TaskItemDto MapToTaskItemDto(TaskItem task) => new TaskItemDto()
    {
        Title = task.Title,
        Description = task.Description,
        AssigneeUsername = task.Assignee?.Username ?? string.Empty,
        Status = task.Status,
        CreatedAt = task.CreatedAt
    };
}