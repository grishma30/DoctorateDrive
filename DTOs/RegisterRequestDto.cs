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

        // Remove [Required] to make it optional
        public string? MobileNumber { get; set; }
    }
}
