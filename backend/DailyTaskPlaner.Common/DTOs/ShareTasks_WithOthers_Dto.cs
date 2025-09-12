namespace DailyTaskPlaner.Common.DTOs;

public class ShareTasks_WithOthers_Dto
{
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public List<int> TaskIds { get; set; }
}
