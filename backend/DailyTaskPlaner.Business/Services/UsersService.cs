using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data;
using DailyTaskPlaner.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DailyTaskPlaner.Business.Services;

public class UsersService(AppDbContext _context, PasswordHasher<User> passwordHasher) : IUsersService
{
    public async Task<User?> GetUserAsync(int id)
    {
        User? user = await _context.Users.FindAsync(id);
        
        return user;
    }

    public async Task<List<User>> GetAllUsers()
    {
        List<User> result = await _context.Users.ToListAsync();
        return result;
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserDto updatedUser)
    {
        var existingUser = await _context.Users.FindAsync(id);

        if (existingUser == null || updatedUser == null)
            return false;

        existingUser.Name = updatedUser.Name;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Username = updatedUser.Username;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> SearchUserAsync(string? username, string? email)
    {
        try
        {
            return await _context.Users
               .FirstOrDefaultAsync(u =>
                   (username != null && u.Username.ToLower() == username.ToLower()) ||
                   (email != null && u.Email.ToLower() == email.ToLower()));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error searching for user: {0}", ex.Message);
            return null;
        }
    }

}
