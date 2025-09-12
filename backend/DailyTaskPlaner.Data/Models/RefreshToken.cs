namespace DailyTaskPlaner.Data.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public int UserId { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
    public User User { get; set; }
}
