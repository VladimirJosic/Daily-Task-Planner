namespace DailyTaskPlaner.Common.DTOs;

public class QueryLLMRequestDto
{
    public string Query { get; set; }
    public DateOnly? Date { get; set; } = null;
}
