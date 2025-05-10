using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Common.Result;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public UserService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<Result<User?>> GetUserByUsername(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user.IsFailure)
        {
            return Result<User?>.Failure(user.Error);
        }

        return Result<User?>.Ok(user.Value);
    }

    public async Task<Result<User>> RegisterAsync(string username, string password)
    {
        try
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser.IsFailure)
            {
                return Result<User>.Failure(existingUser.Error);
            }

            if (existingUser.Value is not null)
            {
                return Result<User>.Failure(Error.New($"User {username} already exist",null, KnownApplicationErrorEnum.UserAlreadyExist));
            }
            
            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };
            
            await _userRepository.AddAsync(user);
            return Result<User>.Ok(user);
        }
        catch (Exception e)
        {
            return Result<User>.Failure(Error.New("An error occurred while registering the user", e, KnownApplicationErrorEnum.SqlGenericError));
        }
    }

    public async Task<Result<string>> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user.IsFailure)
        {
            return Result<string>.Failure(user.Error);
        }

        if (user.Value is null)
        {
            return Result<string>.Failure(Error.New("User not found", null, KnownApplicationErrorEnum.UserNotFound));
        }

        try
        {
            using var hmac = new HMACSHA512(user.Value.PasswordSalt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            if (!hash.SequenceEqual(user.Value.PasswordHash)) return null;

            return Result<string>.Ok(GenerateJwtToken(user.Value));
        }
        catch (Exception e)
        {
            return Result<string>.Failure(Error.New("An error occurred while logging in", e, KnownApplicationErrorEnum.SqlGenericError));
        }
    }
    
    public async Task<Result<IEnumerable<User>>> GetUsers()
    {
        var users = await _userRepository.GetUsersAsync();
        if (users.IsFailure)
        {
            return Result<IEnumerable<User>>.FromFailure(users);
        }

        return users;
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config.GetSection("Jwt:Key").Value!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", user.Username) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}