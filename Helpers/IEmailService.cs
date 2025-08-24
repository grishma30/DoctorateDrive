namespace DoctorateDrive.Helpers
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task<string> GetOtpEmailBodyAsync(string userName, string otpCode);
    }
}
