using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
    Task SendBulkAsync(IEnumerable<string> toEmails, string subject, string body, CancellationToken ct = default);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        var host = _config["Email:Smtp:Host"];
        var portStr = _config["Email:Smtp:Port"];
        var user = _config["Email:Smtp:User"];
        var password = _config["Email:Smtp:Password"];
        var from = _config["Email:Smtp:From"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user))
        {
            _logger.LogWarning("Email not sent to {Email}: Email:Smtp is not configured", toEmail);
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
            _logger.LogInformation("Email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        }
    }

    public async Task SendBulkAsync(IEnumerable<string> toEmails, string subject, string body, CancellationToken ct = default)
    {
        const int batchSize = 20;

        foreach (var batch in toEmails.Chunk(batchSize))
        {
            foreach (var email in batch)
                await SendAsync(email, subject, body, ct);

            await Task.Delay(TimeSpan.FromSeconds(1), ct);
        }
    }
}