using DailyTaskPlaner.Business.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace DailyTaskPlaner.Business.Services;

public sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var smtp = _configuration.GetSection("Smtp");

        var host = smtp["Host"] ?? throw new InvalidOperationException("Smtp:Host missing");
        var port = int.Parse(smtp["Port"] ?? "587");
        var senderEmail = smtp["SenderEmail"] ?? throw new InvalidOperationException("Smtp:SenderEmail missing");
        var pass = smtp["Pass"] ?? throw new InvalidOperationException("Smtp:Pass missing");
        var useStartTls = bool.Parse(smtp["UseStartTls"] ?? "true");

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(senderEmail));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = subject;

        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();

        // Gmail: StartTLS on 587
        var secure = useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

        await client.ConnectAsync(host, port, secure);
        await client.AuthenticateAsync(senderEmail, pass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
