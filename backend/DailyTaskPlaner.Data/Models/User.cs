using System.ComponentModel.DataAnnotations;

namespace DailyTaskPlaner.Data.Models;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string Username { get; set; }

    [EmailAddress]
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public ICollection<DailyTask> OwnedTasks { get; set; } = new List<DailyTask>();
    public ICollection<SharedTask> SharedTasks { get; set; } = new List<SharedTask>();
}
