using System.Net;
using System.Net.Mail;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.Notifications;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
}

public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        var host = config["Email:Smtp:Host"];
        var portStr = config["Email:Smtp:Port"];
        var user = config["Email:Smtp:User"];
        var password = config["Email:Smtp:Password"];
        var from = config["Email:Smtp:From"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user))
        {
            logger.LogWarning("Email not sent to {Email}: Email:Smtp is not configured", toEmail);
            return;
        }

        var port = int.TryParse(portStr, out var p) ? p : 587;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, password),
            EnableSsl = true
        };

        using var message = new MailMessage(from ?? user, toEmail, subject, body)
        {
            IsBodyHtml = false
        };

        try
        {
            await client.SendMailAsync(message, ct);
            logger.LogInformation("Email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        }
    }

   
}