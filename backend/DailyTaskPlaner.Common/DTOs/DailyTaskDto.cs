namespace DailyTaskPlaner.Common.DTOs;

public class DailyTaskDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsUrgent { get; set; }
    public int UserId { get; set; }
}
