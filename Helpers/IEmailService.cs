using System.Threading.Tasks;

namespace DoctorateDrive.Helpers
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task<string> GetOtpEmailBodyAsync(string userName, string otpCode);
        Task<bool> SendOtpEmailAsync(string toEmail, string otp, string userName);
    }
}
