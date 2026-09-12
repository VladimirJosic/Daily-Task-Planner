using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyTaskPlaner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }


    [HttpGet("get-all")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        List<User> users = await _usersService.GetAllUsers();
        
        return Ok(users);
    }

    [HttpGet("Search_user")]
    [Authorize]
    public async Task<IActionResult> SearchUser([FromQuery] string? username, [FromQuery] string? email)
    {
        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("You must provide either a username or an email.");
        }

        var user = await _usersService.SearchUserAsync(username, email);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUser(int id)
    {
        User? user = await _usersService.GetUserAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updatedUser)
    {
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null) 
            return Unauthorized();

        var userId = int.Parse(idClaim.Value);

        if (updatedUser == null)
        {
            return BadRequest("Invalid user data.");
        }

        var result = await _usersService.UpdateUserAsync(userId, updatedUser);

        if (!result)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        return NoContent(); // 204 No Content – success
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
            return Unauthorized();

        var userId = int.Parse(idClaim.Value);
        var user = await _usersService.GetUserAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
}
