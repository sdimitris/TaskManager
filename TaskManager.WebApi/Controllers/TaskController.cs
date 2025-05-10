using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Requests;

namespace TaskManager.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        ArgumentNullException.ThrowIfNull(_logger = logger);
        ArgumentNullException.ThrowIfNull(_taskService = taskService);
    }

    /// <summary>
    /// Fetch all the tasks from the database
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _taskService.GetAllTasksAsync();
        if (result.IsFailure)
        {
            _logger.LogError(result.Error.GetErrorInDetail());
            return Problem(result.Error.GetError(), statusCode: result.Error.ErrorCode);
        }
        
        return Ok(result.Value);
    }

    /// <summary>
    /// Fetch the task with the given id
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _taskService.GetTaskByIdAsync(id);
        if (result.IsFailure)
        {
            _logger.LogError(result.Error.GetErrorInDetail());
            return Problem(result.Error.GetError(), statusCode: result.Error.ErrorCode);
        }
        return Ok(result.Value);
    }
    
    /// <summary>
    /// Creates a task
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateTaskItemRequest task)
    {
        var result = await _taskService.CreateTaskAsync(task);
        if (result.IsFailure)
        {
            _logger.LogError(result.Error.GetErrorInDetail());
            return Problem(result.Error.GetError(), statusCode: result.Error.ErrorCode);
        }
        
        _logger.LogInformation("Task created successfully.");
        return Ok(result.Value);    
    }

    /// <summary>
    /// Updates a task
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, UpdateTaskItemRequest taskItem)
    {
        var result = await _taskService.UpdateTaskAsync(id, taskItem);
        if (result.IsFailure)
        { 
            _logger.LogError(result.Error.GetErrorInDetail());
            return Problem(result.Error.GetError(), statusCode: result.Error.ErrorCode);
        }
        
        return Ok(new { message = "Task updated successfully." });
    }
}