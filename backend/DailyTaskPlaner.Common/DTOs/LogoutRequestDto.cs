using System.ComponentModel.DataAnnotations;

namespace DailyTaskPlaner.Common.DTOs;

public class LogoutRequestDto
{
    [Required]
    public string RefreshToken { get; set; }
}
