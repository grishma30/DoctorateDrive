using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class GenerateOtpRequestDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string EmailId { get; set; } = string.Empty;

    }
}
