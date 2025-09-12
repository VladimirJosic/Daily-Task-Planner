namespace DailyTaskPlaner.Data.Models;

public class DailyTask
{
    public DailyTask() {}
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsUrgent { get; set; }
    public bool IsActive { get; set; } = true;
    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<SharedTask> SharedWith { get; set; } = new List<SharedTask>();
}
