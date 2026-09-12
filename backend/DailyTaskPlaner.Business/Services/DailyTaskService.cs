using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Common.Enums;
using DailyTaskPlaner.Data;
using DailyTaskPlaner.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyTaskPlaner.Business.Services;

public class DailyTaskService(AppDbContext _context) : IDailyTaskService
{

    public async Task<List<DailyTask>> GetAllTasksByUserId(int userId)
    {
        return await _context.DailyTasks
                        .Where(t => t.UserId == userId)
                        .ToListAsync();
    }

    public async Task<List<DailyTask>> GetTasksByUserIdAndEndDate(int userId, DateOnly endDate)
    {
        DateTime endDateTime = endDate.ToDateTime(TimeOnly.MinValue);

        return await _context.DailyTasks
                .Where(t => t.UserId == userId && t.EndDate <= endDateTime)
                .ToListAsync();
    }

    public async Task<List<DailyTask>> GetAllTasksByUserId_WithDateRange(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.DailyTasks
            .Where(t => t.UserId == userId &&
                        t.StartDate >= startDate &&
                        t.StartDate <= endDate &&
                        t.IsActive == true)
            .ToListAsync();
    }

    public async Task<DailyTask?> GetTaskByIdAsync(int id, int userId)
    {
        return await _context.DailyTasks
                        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<DailyTask> CreateTaskAsync(DailyTaskDto task, int userId)
    {
        var testTask = new DailyTask();

        testTask.Title = task.Title;
        testTask.Description = task.Description;
        testTask.StartDate = task.StartDate;
        testTask.EndDate = task.EndDate;
        testTask.IsUrgent = task.IsUrgent;
        testTask.UserId = userId;

        await _context.DailyTasks.AddAsync(testTask);
        await _context.SaveChangesAsync();
        return testTask;
    }

    public async Task<ResultPackage<bool>> DeleteTaskAsync(int id, int userId)
    {
        DailyTask? task = await _context.DailyTasks.FindAsync(id);
        if (task == null || task.UserId != userId)
        {
            return new ResultPackage<bool>(false, ResultStatus.NotFound, $"Task with Id {id} not found");
        }

        try
        {
            _context.DailyTasks.Remove(task);
            await _context.SaveChangesAsync();
            return new ResultPackage<bool>(true, ResultStatus.OK, $"Task with Id {id} successfully deleted");
        }
        catch (Exception)
        {
            return new ResultPackage<bool>(false, ResultStatus.InternalServerError, $"Error updating the database entry");
        }
    }


    public async Task<ResultPackage<bool>> UpdateTaskAsync(int id, DailyTaskDto task, int userId)
    {
        var existingTask = await _context.DailyTasks.FindAsync(id);
        if (existingTask is null || existingTask.UserId != userId)
        {
            return new ResultPackage<bool>(false, ResultStatus.NotFound, $"Task with Id {id} not found");
        }

        if (task is null)
        {
            return new ResultPackage<bool>(false, ResultStatus.BadRequest, $"DailyTaskDto is null");
        }

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.StartDate = task.StartDate;
        existingTask.EndDate = task.EndDate;
        existingTask.IsUrgent = task.IsUrgent;

        try
        {
            await _context.SaveChangesAsync();
            return new ResultPackage<bool>(true, ResultStatus.OK, $"Task with Id {id} successfully updated");
        }
        catch (Exception)
        {
            return new ResultPackage<bool>(false, ResultStatus.InternalServerError, $"Error updating the database entry");
        }
    }


    // SHARE TASKS

    public async Task<ShareTaskResult> ShareTaskAsync(int userId, int friendId, List<int> taskIds)
    {
        // Validate user exists
        User? user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return ShareTaskResult.UserNotFound;
        }

        // Validate friend exists (fixed the userId -> friendId typo)
        User? friend = await _context.Users.FindAsync(friendId);
        if (friend == null)
        {
            return ShareTaskResult.FriendNotFound;
        }

        // Get all tasks to be shared (must be owned by the caller)
        var tasks = await _context.DailyTasks
            .Where(t => t.UserId == userId && taskIds.Contains(t.Id))
            .ToListAsync();

        // Check if all tasks were found
        if (tasks.Count != taskIds.Count)
        {
            return ShareTaskResult.TaskNotFound;
        }

        // Check for already shared tasks
        var existingSharedTasks = await _context.SharedTasks
            .Where(st => taskIds.Contains(st.DailyTaskId) && st.FriendId == friendId)
            .ToListAsync();

        if (existingSharedTasks.Any())
        {
            return ShareTaskResult.AlreadyShared;
        }

        var sharedTasks = tasks.Select(task => new SharedTask
        {
            DailyTaskId = task.Id,
            UserId = userId,
            FriendId = friendId,
        }).ToList();

        try
        {
            await _context.AddRangeAsync(sharedTasks);
            await _context.SaveChangesAsync();

            return ShareTaskResult.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ShareTaskResult.DatabaseError;
        }
    }

    public async Task<List<GetSharedTaskDto>> GetAllSharedTasksAsync(int userId)
    {
        // First check if user exists
        User? user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return null;
        }

        var sharedTasks = await _context.SharedTasks
            .Include(st => st.DailyTask)
            .Where(st => st.UserId == userId || st.FriendId == userId)
            .ToListAsync();

        if (sharedTasks == null || !sharedTasks.Any())
        {
            return new List<GetSharedTaskDto>();
        }

        var result = sharedTasks.Select(st => new GetSharedTaskDto
        {
            Id = st.DailyTaskId,
            UserId = st.UserId,
            FriendId = st.FriendId,
            Title = st.DailyTask.Title,
            Description = st.DailyTask.Description,
            StartDate = st.DailyTask.StartDate,
            EndDate = st.DailyTask.EndDate,
            IsUrgent = st.DailyTask.IsUrgent,
        }).ToList();

        return result;
    }

    public async Task<ResultPackage<bool>> DeleteSharedTaskAsync(int id, int userId)
    {
        SharedTask? task = await _context.SharedTasks
            .FirstOrDefaultAsync(st => st.DailyTaskId == id && st.FriendId == userId);
        if (task == null)
        {
            return new ResultPackage<bool>(false, ResultStatus.NotFound, $"Shared task with Id {id} not found");
        }

        try
        {
            _context.SharedTasks.Remove(task);
            await _context.SaveChangesAsync();
            return new ResultPackage<bool>(true, ResultStatus.OK, $"Shared task with Id {id} successfully deleted");
        }
        catch (Exception)
        {
            return new ResultPackage<bool>(false, ResultStatus.InternalServerError, $"Error updating the database entry");
        }
    }

    public async Task<ResultPackage<bool>> LogicalDeleteTaskAsync(int id, int userId)
    {
        DailyTask? task = await _context.DailyTasks.FindAsync(id);
        if (task == null || task.UserId != userId)
        {
            return new ResultPackage<bool>(false, ResultStatus.NotFound, $"Task with Id {id} not found");
        }

        try
        {
            task.IsActive = false; 
            await _context.SaveChangesAsync();
            return new ResultPackage<bool>(true, ResultStatus.OK, $"Task with Id {id} successfully logically deleted");
        }
        catch (Exception)
        {
            return new ResultPackage<bool>(false, ResultStatus.InternalServerError, $"Error updating the database entry");
        }
    }

    public async Task<List<DailyTask>> SearchDailyTaskAsync(int userId, string? inputQuery, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.DailyTasks
            .Where(t => t.UserId == userId)
            .AsQueryable();

        var searchTerm = inputQuery?.Trim();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t =>
                t.Title.Contains(searchTerm) ||
                t.Description.Contains(searchTerm));
        }

        // Apply date filters
        if (startDate.HasValue)
        {
            query = query.Where(t => t.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.EndDate <= endDate.Value);
        }

        return await query.ToListAsync();
    }
}
