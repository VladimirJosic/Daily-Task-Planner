using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskPlaner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IEmailService emailService, IConfiguration configuration) : ControllerBase
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

        var result = await authService.RequestPasswordResetAsync(email);

        if (result.Status == ResultStatus.NotFound || result.Data is null)
        {
            return NotFound("User not found");
        }

        try
        {
            string resetLink = BuildResetLink(result.Data);

            string emailSubject = "Reset your Daily Task Planner password";
            string emailBody = "Open the link below to choose a new password:\n\n" +
                               resetLink + "\n\n" +
                               "The link is valid for 30 minutes and can be used once. " +
                               "If you did not ask for this, ignore this message: " +
                               "your password stays as it is.";

            await emailService.SendEmailAsync(email, emailSubject, emailBody);
            return Ok(new { Message = "Password reset email sent successfully" });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "An error occurred while processing your request. Failed to send email.");
        }
    }

    [HttpPost("set-new-password")]
    public async Task<IActionResult> SetNewPassword([FromBody] SetNewPasswordDto request)
    {
        var result = await authService.SetNewPasswordAsync(request);

        return result.Status switch
        {
            ResultStatus.OK => Ok(new { Message = result.Message }),
            ResultStatus.BadRequest => BadRequest(result.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Message)
        };
    }

    /// <summary>
    /// The link points at the client application, not at this API, because the user
    /// has to be shown a form before anything is changed.
    /// </summary>
    private string BuildResetLink(string token)
    {
        string clientUrl = configuration["App:ClientUrl"]?.TrimEnd('/')
                           ?? throw new InvalidOperationException("App:ClientUrl missing");

        return $"{clientUrl}/reset-password?token={Uri.EscapeDataString(token)}";
    }
}
