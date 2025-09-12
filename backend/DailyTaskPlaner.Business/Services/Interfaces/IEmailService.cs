namespace DailyTaskPlaner.Business.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string recipient, string subject, string body);
}
