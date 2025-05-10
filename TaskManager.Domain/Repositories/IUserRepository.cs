using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Repositories;

public interface IUserRepository
{
    Task<Result<User?>> GetByUsernameAsync(string username);
    Task<Result> AddAsync(User user);
    Task<Result<IEnumerable<User>>> GetUsersAsync();
}