using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data.Models;

namespace DailyTaskPlaner.Business.Services.Interfaces;

public interface IUsersService
{
    public Task<ResultPackage<User>> CreateUserAsync(CreateUserDto newUser);
    public Task<User?> GetUserAsync(int id);
    public Task<List<User>> GetAllUsers();
    Task<bool> UpdateUserAsync(int id, UpdateUserDto updatedUser);
    public Task<User?> SearchUserAsync(string? username, string? email);
}
