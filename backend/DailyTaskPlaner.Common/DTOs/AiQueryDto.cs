namespace DailyTaskPlaner.Common.DTOs
{
    public enum TimeRange
    {
        Day,
        Week
    }
    public class AiQueryDto
    {
        public string? Question { get; set; }
        public TimeRange Range { get; set; }
    }
}
