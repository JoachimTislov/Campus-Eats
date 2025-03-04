using MailKit.Net.Smtp;
using MimeKit;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;

public class EmailService : IEmailService
{
    public async Task<bool> SendMailAsync(string toEmail, string subject, string mailBody)
    {
        var emailHost = "campuseatsatuis@gmail.com";
        var emailHostPassword = Environment.GetEnvironmentVariable("AppPassword");
        if (emailHostPassword == null) return false;

        var message = CreateMessage(emailHost, toEmail, subject, mailBody);

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailHost, emailHostPassword);
        var result = await client.SendAsync(message);
        await client.DisconnectAsync(true);

        return result.Split(' ')[1] == "OK";
    }

    private static MimeMessage CreateMessage(string emailHost, string toEmail, string subject, string mailBody)
    {
        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(emailHost));
        message.To.Add(MailboxAddress.Parse(toEmail));

        message.Subject = subject;

        var builder = new BodyBuilder
        {
            TextBody = mailBody
        };

        message.Body = builder.ToMessageBody();

        return message;
    }
}