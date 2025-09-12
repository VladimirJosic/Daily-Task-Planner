using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Common.Enums;
using DailyTaskPlaner.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskPlaner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DailyTaskController : ControllerBase
{
    private readonly IDailyTaskService _dailyTaskService;
    public DailyTaskController(IDailyTaskService dailyTaskService)
    {
        _dailyTaskService = dailyTaskService;
    }


    [HttpGet("get-all/{userId}")]
    public async Task<IActionResult> GetAllTasks(int userId)
    {
        var tasks = await _dailyTaskService.GetAllTasksByUserId(userId);
        return Ok(tasks);
    }

    [HttpGet("get-all-date-range/{userId}")]
    public async Task<IActionResult> GetAllTasks(int userId, DateTime startDate, DateTime endDate)
    {
        var tasks = await _dailyTaskService.GetAllTasksByUserId_WithDateRange(userId, startDate, endDate);
        return Ok(tasks);
    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateTask([FromBody] DailyTaskDto task)
    {
        if (task == null)
        {
            return BadRequest("Task cannot be null");
        }
        var createdTask = await _dailyTaskService.CreateTaskAsync(task);
        return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id },createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] DailyTaskDto task)
    {
        if (task == null || id == 0)
        {
            return BadRequest("Task data is invalid");
        }

        ResultPackage<bool> updated = await _dailyTaskService.UpdateTaskAsync(id, task);
        if (updated.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        ResultPackage<bool> deleted = await _dailyTaskService.DeleteTaskAsync(id);
        if (deleted.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("test-get-all")]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _dailyTaskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("test/{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var task = await _dailyTaskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    // SHARE TASKS

    /// <summary>
    /// Share multiple tasks with another user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>

    [HttpPost("share")]
    public async Task<IActionResult> ShareTasks([FromBody] ShareTasks_WithOthers_Dto request)
    {
        ShareTaskResult result = await _dailyTaskService.ShareTaskAsync(request.UserId, request.UserId, request.TaskIds);
        
        switch (result)
        {
            case ShareTaskResult.TaskNotFound:
                return NotFound("Task not found");
            case ShareTaskResult.FriendNotFound:
                return NotFound("Friend not found");
            case ShareTaskResult.AlreadyShared:
                return BadRequest("Task already shared with this friend");
            case ShareTaskResult.Success:
                return Ok();
            default:
                return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("get-all-shared")]
    public async Task<IActionResult> GetAllSharedTasks(int userId)
    {
        List<GetSharedTaskDto>  sharedTasks = await _dailyTaskService.GetAllSharedTasksAsync(userId);
        if (sharedTasks is null)
        {
            return NotFound();
        }

        return Ok(sharedTasks);
    }


    [HttpPut("deactivate/{id}")]
    public async Task<IActionResult> LogicalDeleteTask(int id)
    {
        ResultPackage<bool> deleted = await _dailyTaskService.LogicalDeleteTaskAsync(id);
        if (deleted.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<DailyTask>>> SearchTasks(
        [FromQuery] int userId,
        [FromQuery] string? inputQuery = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var result = await _dailyTaskService.SearchDailyTaskAsync(
            userId,
            inputQuery,
            startDate,
            endDate);

        return Ok(result);
    }
}
