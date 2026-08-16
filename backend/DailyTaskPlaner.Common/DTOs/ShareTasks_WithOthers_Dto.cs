namespace DailyTaskPlaner.Common.DTOs;

public class ShareTasks_WithOthers_Dto
{
    public int FriendId { get; set; }
    public List<int> TaskIds { get; set; }
}
