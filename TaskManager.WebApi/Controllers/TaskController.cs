using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Common.Enums;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Requests;

namespace TaskManager.Controllers;

[ApiController]
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
            _logger.LogError(result.Error.GetError());
            return Problem(title: "Something went wrong while fetching the tasks", statusCode: StatusCodes.Status400BadRequest);
        }
        
        return Ok(result.Value);
    }

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
            if (result.Error.ApplicationError.Equals(KnownApplicationErrorEnum.TaskNotFound))
                return Problem(title: $"Task with id {id} not found", statusCode: 404);
            
            _logger.LogError(result.Error.GetError());
            return Problem(title: $"Something went wrong while getting the task with id: {id}", statusCode: 400);
        }
        return Ok(result.Value);
    }

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
            _logger.LogError(result.Error.GetError());
            return Problem(title: "Something went wrong while creating the task", statusCode: 400);
        }
        
        _logger.LogInformation("Task created successfully.");
        return Ok(result.Value);    
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, TaskItem task)
    {
        if (id != task.Id)
            return Problem("Task ID mismatch");

        var result = await _taskService.UpdateTaskAsync(task);
        if (result.IsFailure)
        { 
            if (result.Error.ApplicationError.Equals(KnownApplicationErrorEnum.TaskNotFound))
                return Problem(title: $"Task with id {id} not found", statusCode: 404);
            
            _logger.LogError(result.Error.GetError());
            return Problem(title: $"Something went wrong while updating the task with id {id}", statusCode: 400);
        }
        
        return Ok(new { message = "Task updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);
        if (result.IsFailure)
        {
            if (result.Error.ApplicationError.Equals(KnownApplicationErrorEnum.TaskNotFound))
                return Problem(title: $"Task with id {id} not found", statusCode: 404);
            
            _logger.LogError(result.Error.GetError());
            return Problem(title: $"Something went wrong while deleting the task with id {id}", statusCode: 400);
        }
        
        return Ok(new { message = "Task deleted successfully." });
    }
}