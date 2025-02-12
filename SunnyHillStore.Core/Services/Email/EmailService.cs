using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Password Reset Request - Sunny Hill Store";

        var builder = new BodyBuilder();
        builder.HtmlBody = $@"
            <html>
                <body style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #333;'>Password Reset Request</h2>
                    <p>Hello,</p>
                    <p>We received a request to reset your password for your Sunny Hill Store account.</p>
                    <p>To reset your password, please click the button below:</p>
                    <p style='text-align: center;'>
                        <a href='{resetLink}' 
                           style='background-color: #4CAF50; 
                                  color: white; 
                                  padding: 12px 12px; 
                                  text-decoration: none; 
                                  border-radius: 4px;
                                  display: inline-block;'>
                            Reset Password
                        </a>
                    </p>
                    <p>If you didn't request this password reset, please ignore this email.</p>
                    <p>This link will expire in 1 hour for security reasons.</p>
                    <p>Best regards,<br>Sunny Hill Store Team</p>
                </body>
            </html>";

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
} 