using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class TokenValidationDto
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
    }
}
