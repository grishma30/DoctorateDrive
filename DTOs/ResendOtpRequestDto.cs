using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class ResendOtpRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Valid email address is required")]
        public string EmailId { get; set; } = string.Empty;
    }
}
