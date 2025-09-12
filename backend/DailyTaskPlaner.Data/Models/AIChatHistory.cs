using Microsoft.Extensions.AI;

namespace DailyTaskPlaner.Data.Models;

public class AIChatHistory
{
    public int Id { get; set; }
    public string Role { get; set; } // "user" or "assistant"
    public string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public bool IsDeleted { get; set; } = false;
}
