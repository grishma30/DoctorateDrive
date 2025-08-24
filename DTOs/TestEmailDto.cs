using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class TestEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Valid email address is required")]
        public string Email { get; set; } = string.Empty;
    }
}
