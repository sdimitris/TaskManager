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

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        await _taskRepository.AddAsync(task);
        return task;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        var existingTask = await _taskRepository.GetByIdAsync(task.Id);
        if (existingTask == null)
            return false;

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Status = task.Status;
        existingTask.AssigneeId = task.AssigneeId;

        await _taskRepository.UpdateAsync(existingTask);
        return true;
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return false;

        await _taskRepository.DeleteAsync(id);
        return true;
    }
}