using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data.Models;

namespace DailyTaskPlaner.Business.Services.Interfaces;

public interface IAuthService
{
    Task<ResultPackage<User>> RegisterAsync(UserDto request);               
    Task<LoginResponseDto?> LoginAsync(LoginUserDto request);
    Task<ResultPackage<bool>> LogoutAsync(string refreshToken);
    Task<ResultPackage<TokenResponseDto?>> RefreshTokensAsync(RefreshTokenRequestDto request);
    Task<ResultPackage<string>> RequestPasswordResetAsync(string email);
    Task<ResultPackage<bool>> SetNewPasswordAsync(SetNewPasswordDto request);
}