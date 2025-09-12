using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Common.Enums;
using DailyTaskPlaner.Data.Models;

namespace DailyTaskPlaner.Business.Services.Interfaces;

public interface IDailyTaskService
{
    Task<DailyTask?> GetTaskByIdAsync(int id);
    Task<List<DailyTask>> GetAllTasksAsync();
    Task<List<DailyTask>> GetAllTasksByUserId(int userId);
    Task<List<DailyTask>> GetAllTasksByUserId_WithDateRange(int userId, DateTime startDate, DateTime endDate);
    Task<List<DailyTask>> GetTasksByUserIdAndEndDate(int userId, DateOnly endDate);
    Task<DailyTask?> CreateTaskAsync(DailyTaskDto task);
    Task<ResultPackage<bool>> UpdateTaskAsync(int id, DailyTaskDto task);
    Task<ResultPackage<bool>> DeleteTaskAsync(int id);
    Task<ShareTaskResult> ShareTaskAsync(int userId, int friendId, List<int> taskIds);
    Task<List<GetSharedTaskDto>> GetAllSharedTasksAsync(int userId);
    Task<ResultPackage<bool>> LogicalDeleteTaskAsync(int id);
    Task<List<DailyTask>> SearchDailyTaskAsync(int userId, string? inputQuery, DateTime? startDate, DateTime? endDate);


}
