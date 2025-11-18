using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class ResendOtpRequestDto
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string EmailAddress { get; set; } = string.Empty;
    }
}
