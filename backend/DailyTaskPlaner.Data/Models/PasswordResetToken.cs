namespace DailyTaskPlaner.Data.Models;

/// <summary>
/// A single-use permit to set a new password, handed out by email.
///
/// Only the hash of the token is stored. The token itself exists in the link
/// sent to the user and nowhere else, so a leaked database does not let anyone
/// take over an account.
/// </summary>
public class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public required string TokenHash { get; set; }

    public DateTime ExpiresOnUtc { get; set; }

    /// <summary>Set the moment the token is spent, which keeps it single-use.</summary>
    public DateTime? UsedOnUtc { get; set; }

    public User User { get; set; }
}
