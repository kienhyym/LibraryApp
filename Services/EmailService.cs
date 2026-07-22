using LibraryApp.Configurations;
using LibraryApp.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LibraryApp.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _emailSettings = options.Value;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                _emailSettings.SenderName,
                _emailSettings.SenderEmail));

        email.To.Add(
            MailboxAddress.Parse(toEmail));

        email.Subject = subject;

        email.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _emailSettings.SmtpServer,
            _emailSettings.Port,
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _emailSettings.SenderEmail,
            _emailSettings.AppPassword);

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}