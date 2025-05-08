using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _context;

    public TaskItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<TaskItem>>> GetAllAsync()
    {
        try
        {
            var res = await _context.Tasks.Include(t => t.Assignee).ToListAsync();
            return Result<IEnumerable<TaskItem>>.Ok(res);

        }
        catch (Exception e)
        {
            return Result<IEnumerable<TaskItem>>.Failure(Error.New("An error occurred while fetching the tasks from the database", e, KnownApplicationErrorEnum.SqlGenericError));
        }
    }

    public async Task<Result<TaskItem?>> GetByIdAsync(int id)
    {
        try
        {
            var task = await _context.Tasks.Include(t => t.Assignee).FirstOrDefaultAsync(t => t.Id == id);
            return Result<TaskItem?>.Ok(task);
        }
        catch (Exception e)
        {
            return Result<TaskItem?>.Failure(Error.New($"An error occurred while fetching the task: {id} from the database", e, KnownApplicationErrorEnum.SqlGenericError));
        }
    } 

    public async Task<Result> AddAsync(TaskItem task)
    {
        try
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Failure(Error.New("An error occurred while adding the task to the database", e, KnownApplicationErrorEnum.SqlGenericError));
        }
    }

    public async Task<Result> UpdateAsync(TaskItem task)
    {
        try
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Failure(Error.New("An error occurred while updating the task to the database", e,
                KnownApplicationErrorEnum.SqlGenericError));
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is not null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.New("An error occurred while deleting the task from the database", ex,
                KnownApplicationErrorEnum.SqlGenericError));
        }
    }
}