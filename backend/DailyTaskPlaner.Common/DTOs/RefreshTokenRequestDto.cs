namespace DailyTaskPlaner.Common.DTOs;

public class RefreshTokenRequestDto {
    public int UserId { get; set; } // Treba biti `int`, jer je `User.Id` int
    public string RefreshToken { get; set; }
}
