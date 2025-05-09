using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Requests;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(_userService = userService);
        ArgumentNullException.ThrowIfNull(_logger = logger);
    }
    
    
    // POST: api/User
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(RegisterUserRequest user)
    {
        var createdUserResult = await _userService.RegisterAsync(user.Username, user.Password);
        if (createdUserResult.IsFailure)
        {
            if (createdUserResult.Error.ApplicationError.Equals(KnownApplicationErrorEnum.UserAlreadyExist))
            {
                return Problem(title: $"User {user.Username} already exist", statusCode: StatusCodes.Status409Conflict);
            }
            
            _logger.LogError(createdUserResult.Error.GetError());
            return Problem(title: $"Something went wrong while creating user", statusCode: StatusCodes.Status400BadRequest);

        }
        
        return Ok(new { message = "User created successfully." });
    }
    
    // POST: api/User/login
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] RegisterUserRequest loginRequest)
    {
        var loginResult = await _userService.LoginAsync(loginRequest.Username, loginRequest.Password);
        if (loginResult.IsFailure)
        {
            if (loginResult.Error.ApplicationError.Equals(KnownApplicationErrorEnum.UserNotFound))
            {
                return Problem(title: $"User {loginRequest.Username} not found", statusCode: StatusCodes.Status404NotFound);
            }
            
            _logger.LogError(loginResult.Error.GetError());
            return Problem(title: $"Something went wrong while logging in", statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(new { Token = loginResult.Value });
    }
}