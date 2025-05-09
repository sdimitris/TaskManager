using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces;

public interface IUserService
{
    Task<Result<User?>> GetUserByUsername(string username);
    Task<Result<string>> RegisterAsync(string username, string password);
    Task<Result<string>> LoginAsync(string username, string password);
}