using DailyTaskPlaner.Common.DTOs;

namespace DailyTaskPlaner.Business.Services.Interfaces;

public interface IAiService
{
    Task<string> AskAboutTaskAsync(AiQueryDto dto);
}
