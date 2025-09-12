namespace DailyTaskPlaner.Common.DTOs;

public class QueryLLMRequestDto
{
    public int UserId { get; set; }
    public string Query { get; set; }
    public DateOnly? Date { get; set; } = null;
}
