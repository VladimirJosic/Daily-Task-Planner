using DailyTaskPlaner.Business.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace DailyTaskPlaner.Business.Services;

public sealed class EmailService : IEmailService
{
    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        const string senderEmail = "leglozmijica@gmail.com";
        const string senderPassword = "llxq nrck hrhh ksfp";
        const string server = "smtp.gmail.com";
        const int port = 587;

        using (var client = new SmtpClient(server, port))
        {
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            client.EnableSsl = true;

            try
            {
                await client.SendMailAsync(new MailMessage(senderEmail, recipient, subject, body));
            }
            catch (SmtpException)
            {
                throw;
            }
        }
    }
}
