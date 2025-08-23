using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string EmailOrMobile { get; set; } = string.Empty;

        [Required]
        public string OtpCode { get; set; } = string.Empty;  // Using OtpCode, not Otp
    }
}
