using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DoctorateDrive.Helpers
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(
                        _emailSettings.SenderEmail,
                        _emailSettings.Password
                    );

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email sending failed: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetOtpEmailBodyAsync(string userName, string otpCode)
        {
            return await Task.FromResult($@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background: #f4f4f4; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #1e5bb8 0%, #2980b9 100%); padding: 30px; text-align: center; color: white; }}
        .content {{ padding: 40px 30px; }}
        .otp-box {{ background: #f8f9fa; border: 2px dashed #1e5bb8; border-radius: 10px; padding: 20px; text-align: center; margin: 30px 0; }}
        .otp-code {{ font-size: 36px; font-weight: bold; color: #1e5bb8; letter-spacing: 8px; }}
        .footer {{ background: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #6c757d; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎓 CHARUSAT PhD Portal</h1>
            <p>Email Verification</p>
        </div>
        <div class='content'>
            <h2>Hello {userName},</h2>
            <p>Please use this OTP to verify your email:</p>
            <div class='otp-box'>
                <div style='color: #6c757d; font-size: 14px; margin-bottom: 10px;'>Your OTP Code</div>
                <div class='otp-code'>{otpCode}</div>
                <div style='color: #6c757d; font-size: 12px; margin-top: 10px;'>Valid for 10 minutes</div>
            </div>
            <p style='color: #dc3545;'>⚠️ Never share this OTP with anyone.</p>
        </div>
        <div class='footer'>
            <p>© 2025 CHARUSAT University. All rights reserved.</p>
        </div>
    </div>
</body>
</html>");
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string otp, string userName)
        {
            try
            {
                var emailBody = await GetOtpEmailBodyAsync(userName, otp);
                await SendEmailAsync(toEmail, "Email Verification - OTP Code", emailBody);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OTP email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
