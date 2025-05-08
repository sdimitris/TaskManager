using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Repositories;

public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
    Task AddAsync(User user);
}