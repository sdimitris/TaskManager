using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Repositories;

public interface ITaskItemRepository
{
    Task<Result<IEnumerable<TaskItem>>> GetAllAsync();
    Task<Result<TaskItem?>> GetByIdAsync(int id);
    Task<Result> AddAsync(TaskItem task);
    Task<Result> UpdateAsync(TaskItem task);
    Task<Result> DeleteAsync(int id);
}
