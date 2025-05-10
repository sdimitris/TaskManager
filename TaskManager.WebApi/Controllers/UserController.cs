using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Common.Result;
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
            _logger.LogError(createdUserResult.Error.GetErrorInDetail());
            return Problem(title: $"Something went wrong while creating user", statusCode: createdUserResult.Error.ErrorCode);

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
            _logger.LogError(loginResult.Error.GetErrorInDetail());
            return Problem(title: $"Something went wrong while logging in", statusCode: loginResult.Error.ErrorCode);
        }

        return Ok(new { Token = loginResult.Value });
    }
    
    // POST: api/User
    [HttpGet("/getUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers()
    {
        var usersResult = await _userService.GetUsers();
        if (usersResult.IsFailure)
        {
            _logger.LogError(usersResult.Error.GetErrorInDetail());
            return Problem(title: $"Something went wrong while getting users", statusCode: usersResult.Error.ErrorCode);
        }

        return Ok(usersResult.Value);
    }
}