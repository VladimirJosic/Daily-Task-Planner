namespace DailyTaskPlaner.Common.DTOs;

public class GetSharedTaskDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; } // what if you are the friend?
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsUrgent { get; set; }
}
