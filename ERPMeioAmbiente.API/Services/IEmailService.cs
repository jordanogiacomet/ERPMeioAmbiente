using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string message);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(toEmail) || !IsValidEmail(toEmail))
        {
            throw new ArgumentException("Invalid email address", nameof(toEmail));
        }

        try
        {
            var apiKey = _configuration["MailgunAPIKey"];
            var domain = _configuration["MailgunDomain"];
            var fromEmail = _configuration["FromEmailAddress"];
            var client = new SmtpClient("smtp.mailgun.org", 587)
            {
                Credentials = new NetworkCredential("postmaster@" + domain, apiKey),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "An error occurred while sending an email.");
            throw;
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
