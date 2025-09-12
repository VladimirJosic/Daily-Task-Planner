namespace DailyTaskPlaner.Common.DTOs;

public class LoginResponseDto
{
    public required int UserId { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
