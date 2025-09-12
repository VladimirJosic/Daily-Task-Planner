namespace DailyTaskPlaner.Data.Models;

public class SharedTask
{
    public int DailyTaskId { get; set; }
    public DailyTask DailyTask { get; set; } = null!;
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public User User { get; set; } = null!; 
}
