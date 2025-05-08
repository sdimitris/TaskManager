using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User?>> GetByUsernameAsync(string username)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            return Result<User?>.Ok(user);
        }
        catch (Exception e)
        {
            return Result<User?>.Failure(Error.New($"An error occurred while fetching the user: {username} from the database", e, KnownApplicationErrorEnum.SqlGenericError));
        }

    }

    public async Task<Result> AddAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();  
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Failure(Error.New("An error occurred while adding the user to the database", e, KnownApplicationErrorEnum.SqlGenericError));
        }
    }
}
