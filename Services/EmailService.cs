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

    #region Send Email

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

    #endregion

    #region Send OTP

    public async Task SendOtpAsync(
        string toEmail,
        string otpCode)
    {
        var subject = "Xác thực tài khoản - Library Management System";

        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family:Arial,sans-serif;background:#f5f5f5;padding:30px;'>

    <div style='max-width:600px;margin:auto;background:#fff;border-radius:10px;padding:30px;'>

        <h2 style='color:#0d6efd;text-align:center;'>
            Library Management System
        </h2>

        <p>Xin chào,</p>

        <p>
            Đây là mã OTP dùng để xác thực email của bạn.
        </p>

        <div style='
            margin:30px auto;
            width:220px;
            text-align:center;
            font-size:34px;
            font-weight:bold;
            letter-spacing:8px;
            color:#0d6efd;
            border:2px dashed #0d6efd;
            padding:18px;'>

            {otpCode}

        </div>

        <p>
            Mã OTP có hiệu lực trong
            <strong>5 phút</strong>.
        </p>

        <p>
            Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
        </p>

        <hr>

        <div style='font-size:12px;color:#777;text-align:center;'>

            © Library Management System

        </div>

    </div>

</body>
</html>";

        await SendEmailAsync(
            toEmail,
            subject,
            htmlBody);
    }

    #endregion
}