namespace TaskManager.Domain.Interfaces;

public interface IUserService
{
    Task<string> RegisterAsync(string username, string password);
    Task<string> LoginAsync(string username, string password);
}