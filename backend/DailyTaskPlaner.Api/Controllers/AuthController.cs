using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskPlaner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IEmailService emailService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(UserDto request)
    {
        var result = await authService.RegisterAsync(request);

        return result.Status switch
        {
            ResultStatus.Created => CreatedAtAction(nameof(Register), new
            {
                user = result.Data,
                message = result.Message
            }),
            ResultStatus.Conflict => Conflict(result.Message),
            ResultStatus.BadRequest => BadRequest(result.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login( LoginUserDto request)
    {
        var result = await authService.LoginAsync(request);
        if (result is null)
            return BadRequest("Invalid username or password.");

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        var result = await authService.LogoutAsync(request.RefreshToken);
        if (result.Status == ResultStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }

        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        ResultPackage<TokenResponseDto?> result = await authService.RefreshTokensAsync(request);
        if (result.Status == ResultStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email address is required");
        }

        string? newPassword = await authService.ResetPassword(email);
        if (newPassword is null)
        {
            return NotFound("User not found");
        }

        try
        {
            string emailSubject = "Your Password Has Been Reset";
            string emailBody = $"Your new temporary password is: {newPassword}\n\n" +
                           "Please change this password immediately after logging in.";

            await emailService.SendEmailAsync(email, emailSubject, emailBody);
            return Ok(new { Message = "Password reset email sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "An error occurred while processing your request. Failed to send email.");
        }
    }
}
