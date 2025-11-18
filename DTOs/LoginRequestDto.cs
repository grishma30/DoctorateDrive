using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "The EmailOrMobile field is required.")]
        public string EmailOrMobile { get; set; } = string.Empty;

        [Required(ErrorMessage = "The OTP code is required.")]
        public string OtpCode { get; set; } = string.Empty;
    }
}
