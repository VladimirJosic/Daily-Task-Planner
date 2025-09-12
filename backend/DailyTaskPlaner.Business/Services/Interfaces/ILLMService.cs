namespace DailyTaskPlaner.Business.Services.Interfaces;

public interface ILLMService
{
    public Task<string> ProcessQuery(int userId, string query, DateOnly? endDate);
    public Task ClearChatHistoryLogical(int userId);
    public Task ClearChatHistoryPhysical(int userId);
}
