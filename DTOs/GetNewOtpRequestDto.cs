using System.ComponentModel.DataAnnotations;

namespace DoctorateDrive.DTOs
{
    public class GetNewOtpRequestDto
    {
        [Required]
        public string EmailOrMobile { get; set; } = string.Empty;
    }
}
