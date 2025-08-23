using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EmailId { get; set; } = string.Empty;

        [Required]
        public string MobileNumber { get; set; } = string.Empty;
    }
}
