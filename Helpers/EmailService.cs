using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DoctorateDrive.Helpers
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment _environment;

        public EmailService(EmailSettings emailSettings, IWebHostEnvironment environment)
        {
            _emailSettings = emailSettings;
            _environment = environment;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task<string> GetOtpEmailBodyAsync(string userName, string otpCode)
        {
            // Read the HTML template file
            var templatePath = Path.Combine(_environment.WebRootPath, "EmailTemplates", "OtpTemplate.html");
            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders with actual values
            var emailBody = htmlTemplate
                .Replace("{{USERNAME}}", userName)
                .Replace("{{OTPCODE}}", otpCode)
                .Replace("{{EXPIRYTIME}}", DateTime.Now.AddMinutes(10).ToString("HH:mm"));

            return emailBody;
        }
    }
}
